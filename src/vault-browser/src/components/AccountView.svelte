<script lang="ts">
  import { onMount } from 'svelte';
  import { user } from '../lib/state/user.svelte';
  import { cart } from '../lib/state/cart.svelte';
  import { ui } from '../lib/state/ui.svelte';
  import { fetchUserOrders } from '../lib/api';
  import type { Order } from '../types';
  import TimelineStepper from './ui/TimelineStepper.svelte';
  import CornerBrackets from './ui/CornerBrackets.svelte';
  import VaultCard from './ui/VaultCard.svelte';
  import VaultButton from './ui/VaultButton.svelte';
  import VaultModal from './ui/VaultModal.svelte';

  let orders = $state<Order[]>([]);
  let loadingOrders = $state(false);
  let orderSort = $state<'newest' | 'delivered' | 'total'>('newest');

  // Login form state
  let loginEmail = $state('');
  let loginName = $state('');
  let loginPhone = $state('');

  // Address modal state
  let newAddressModalOpen = $state(false);
  let newCity = $state('');
  let newLine1 = $state('');
  let newDetails = $state('');
  let newType = $state<'PVZ' | 'Courier' | 'Postal'>('PVZ');

  // Edit profile modal state
  let editProfileModalOpen = $state(false);
  let editName = $state('');
  let editPhone = $state('');

  async function loadUserOrders(email: string) {
    if (!email) {
      orders = [];
      return;
    }
    loadingOrders = true;
    try {
      orders = await fetchUserOrders(email);
    } catch {
      orders = [];
    } finally {
      loadingOrders = false;
    }
  }

  onMount(async () => {
    if (user.isLoggedIn && user.email) {
      await loadUserOrders(user.email);
    }
  });

  async function handleLogin(e: SubmitEvent) {
    e.preventDefault();
    if (!loginEmail.trim()) {
      ui.notify('Please provide your imperial comm-mail (email).', 'crimson');
      return;
    }
    user.login(loginEmail, loginName, loginPhone);
    ui.notify(`Commander credentials sanctioned for ${user.email}`, 'emerald');
    await loadUserOrders(user.email);
  }

  function handleLogout() {
    user.logout();
    orders = [];
    loginEmail = '';
    loginName = '';
    loginPhone = '';
    ui.notify('Comm-link terminated. Commander signed out.', 'crimson');
  }

  function openEditProfile() {
    editName = user.name;
    editPhone = user.phone;
    editProfileModalOpen = true;
  }

  function handleSaveProfile() {
    user.updateProfile(editName, editPhone);
    editProfileModalOpen = false;
    ui.notify('Commander credentials successfully updated in Munitorum registry.', 'emerald');
  }

  const activeOrder = $derived.by(() => {
    if (orders.length === 0) return null;
    const nonDelivered = orders.find(
      o => !o.status.toLowerCase().includes('delivered') && !o.status.toLowerCase().includes('canceled')
    );
    return nonDelivered || orders[0];
  });

  const activeOrderStep = $derived.by(() => {
    if (!activeOrder) return 1;
    const st = activeOrder.status.toLowerCase();
    if (st.includes('delivered')) return 4;
    if (st.includes('transit') || st.includes('shipped') || st.includes('dispatch')) return 3;
    if (st.includes('process') || st.includes('packed') || st.includes('assembl')) return 2;
    return 1;
  });

  const totalSpent = $derived.by(() => {
    return orders
      .filter(o => !o.status.toLowerCase().includes('canceled'))
      .reduce((sum, o) => sum + o.totalAmount, 0);
  });

  const loyaltyTier = $derived.by(() => {
    if (totalSpent >= 600) {
      return {
        name: 'Master of the Forge',
        discount: 10,
        currentMin: 600,
        nextThreshold: 600,
        progress: 100,
        isMax: true
      };
    } else if (totalSpent >= 300) {
      return {
        name: 'Veteran',
        discount: 7,
        currentMin: 300,
        nextThreshold: 600,
        progress: Math.min(100, Math.round(((totalSpent - 300) / 300) * 100)),
        isMax: false
      };
    } else if (totalSpent >= 100) {
      return {
        name: 'Scout',
        discount: 5,
        currentMin: 100,
        nextThreshold: 300,
        progress: Math.min(100, Math.round(((totalSpent - 100) / 200) * 100)),
        isMax: false
      };
    } else {
      return {
        name: 'Initiate',
        discount: 0,
        currentMin: 0,
        nextThreshold: 100,
        progress: Math.min(100, Math.round((totalSpent / 100) * 100)),
        isMax: false
      };
    }
  });

  const sortedOrders = $derived.by(() => {
    let list = [...orders];
    if (orderSort === 'delivered') {
      list = list.filter(o => o.status.toLowerCase().includes('delivered'));
    } else if (orderSort === 'total') {
      list.sort((a, b) => b.totalAmount - a.totalAmount);
    } else {
      list.sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime());
    }
    return list;
  });

  function copyPromo(code: string) {
    if (navigator.clipboard) {
      navigator.clipboard.writeText(code);
    }
    cart.applyPromo(code);
    ui.notify(`Promo code ${code} copied and applied to cart manifest!`, 'emerald');
  }

  function handleRepeatOrder(order: Order) {
    for (const it of order.items) {
      cart.addItem({
        id: it.miniatureId,
        name: it.miniatureName,
        price: it.unitPrice,
        quantity: it.quantity,
        faction: it.faction,
        isStarterSet: it.isStarterSet
      });
    }
    ui.pulseCart();
    ui.notify(`Requisition order #${order.orderNumber} items added to active cart!`, 'gold');
    ui.openCart();
  }

  function handleAddAddress() {
    if (!newCity.trim() || !newLine1.trim()) {
      ui.notify('City and address lines are required.', 'crimson');
      return;
    }
    user.addAddress({
      city: newCity.trim(),
      line1: newLine1.trim(),
      details: newDetails.trim() || 'Standard Imperial Relay',
      type: newType,
      isPrimary: user.addresses.length === 0
    });
    newCity = '';
    newLine1 = '';
    newDetails = '';
    newAddressModalOpen = false;
    ui.notify('New destination registered in the Munitorum index.', 'emerald');
  }
</script>

<!-- BEGIN: BreadcrumbAndBanner matching design/account.html -->
<section class="border-b border-[#38332b] bg-[#0c0d11]/80 py-4" data-purpose="breadcrumbs-and-title">
  <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
    <div class="flex items-center space-x-2 text-[11px] font-cinzel tracking-widest text-[#787265] uppercase mb-1">
      <a href="/" class="hover:text-vault-gold transition-colors cursor-pointer" onclick={(e) => { e.preventDefault(); ui.navigateTo('catalog'); }}>Home</a>
      <span>/</span>
      <a href="/" class="hover:text-vault-gold transition-colors cursor-pointer" onclick={(e) => { e.preventDefault(); ui.navigateTo('catalog'); }}>Catalog</a>
      <span>/</span>
      <span class="text-vault-gold">Commander Dossier</span>
    </div>
    <div class="flex flex-col md:flex-row md:items-center justify-between gap-2">
      <div>
        <h1 class="font-cinzel font-bold text-2xl sm:text-3xl text-[#f2e6cb] tracking-wider uppercase flex items-center gap-3">
          <span>Commander Dossier</span>
        </h1>
        <p class="text-xs text-[#8e897e] tracking-wide mt-1 font-sans">
          Personal credentials, active order tracking, loyalty rewards & order history
        </p>
      </div>
      {#if user.isLoggedIn}
        <div class="flex items-center space-x-2 self-start md:self-auto bg-[#14151a] px-3 py-1.5 border border-[#38332b]">
          <span class="w-2 h-2 rounded-full bg-emerald-500"></span>
          <span class="font-cinzel text-[11px] uppercase tracking-widest text-[#a7a296]">Account Active</span>
        </div>
      {:else}
        <div class="flex items-center space-x-2 self-start md:self-auto bg-[#14151a] px-3 py-1.5 border border-[#38332b]">
          <span class="w-2 h-2 rounded-full bg-amber-500"></span>
          <span class="font-cinzel text-[11px] uppercase tracking-widest text-[#a7a296]">Guest Visitor</span>
        </div>
      {/if}
    </div>
  </div>
</section>
<!-- END: BreadcrumbAndBanner -->

<!-- MAIN CONTENT WRAPPER -->
<main class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8 flex-1 w-full space-y-8" data-purpose="user-dashboard">
  {#if !user.isLoggedIn}
    <!-- GUEST / SIGN-IN VIEW -->
    <div class="max-w-xl mx-auto py-6">
      <VaultCard brackets={true} class="p-8 border-2 border-vault-gold/60 shadow-2xl">
        <div class="text-center mb-6">
          <div class="w-16 h-16 mx-auto mb-4 rounded-full bg-[#181a24] border border-vault-gold flex items-center justify-center shadow-[0_0_15px_rgba(223,185,108,0.3)]">
            <svg class="w-8 h-8 text-vault-gold" fill="currentColor" viewBox="0 0 24 24">
              <path d="M12 2L4 9l2 9 6 3 6-3 2-9-8-7zm0 3.5l5 4.5-1.5 6.5-3.5-1.7V10h-2v4.8L6.5 16.5 5 10l5-4.5z"></path>
            </svg>
          </div>
          <h2 class="font-cinzel font-bold text-xl sm:text-2xl text-vault-brightGold uppercase tracking-widest">
            Commander Identification
          </h2>
          <p class="text-xs text-gray-400 font-sans mt-2">
            Enter your imperial comm-mail to access your sanctioned requisition dossier, track active dispatches, and view loyalty standing.
          </p>
        </div>

        <form class="space-y-4" onsubmit={handleLogin}>
          <div>
            <label class="block text-[11px] font-cinzel text-gray-300 uppercase mb-1" for="loginEmailInput">
              Comm-mail (Email) *
            </label>
            <input
              id="loginEmailInput"
              type="email"
              required
              placeholder="e.g. commander@imperium.vault"
              class="w-full bg-[#0e1017] border border-[#5a482b] focus:border-vault-gold text-xs text-white p-2.5 focus:outline-none font-mono"
              bind:value={loginEmail}
            />
          </div>

          <div>
            <label class="block text-[11px] font-cinzel text-gray-300 uppercase mb-1" for="loginNameInput">
              Callsign / Full Name
            </label>
            <input
              id="loginNameInput"
              type="text"
              placeholder="e.g. Lord Castellan"
              class="w-full bg-[#0e1017] border border-[#5a482b] focus:border-vault-gold text-xs text-white p-2.5 focus:outline-none"
              bind:value={loginName}
            />
          </div>

          <div>
            <label class="block text-[11px] font-cinzel text-gray-300 uppercase mb-1" for="loginPhoneInput">
              Comm-line (Phone)
            </label>
            <input
              id="loginPhoneInput"
              type="tel"
              placeholder="e.g. +7 999 123-45-67"
              class="w-full bg-[#0e1017] border border-[#5a482b] focus:border-vault-gold text-xs text-white p-2.5 focus:outline-none font-mono"
              bind:value={loginPhone}
            />
          </div>

          <VaultButton
            type="submit"
            variant="gold"
            size="lg"
            class="w-full mt-4 shadow-lg hover:shadow-[0_0_15px_rgba(223,185,108,0.3)]"
          >
            Sanctify & Access Dossier
          </VaultButton>
        </form>

        <div class="mt-6 pt-4 border-t border-[#2a261e] text-center">
          <span class="text-[10px] font-cinzel uppercase text-gray-500 tracking-wider block mb-2">
            Sample Inquisitorial Ciphers:
          </span>
          <div class="flex flex-wrap justify-center gap-2">
            <button
              type="button"
              class="text-[10px] font-mono bg-[#141620] border border-[#3b3221] hover:border-vault-gold px-2.5 py-1 text-vault-gold transition-colors cursor-pointer"
              onclick={() => { loginEmail = 'lord.castellan@cadion.vault'; loginName = 'Lord Castellan Creed'; loginPhone = '+7 999 001-40-00'; }}
            >
              lord.castellan@cadion.vault
            </button>
            <button
              type="button"
              class="text-[10px] font-mono bg-[#141620] border border-[#3b3221] hover:border-vault-gold px-2.5 py-1 text-vault-gold transition-colors cursor-pointer"
              onclick={() => { loginEmail = 'archmagos.cawl@mars.vault'; loginName = 'Archmagos Belisarius'; loginPhone = '+7 999 002-00-01'; }}
            >
              archmagos.cawl@mars.vault
            </button>
          </div>
        </div>
      </VaultCard>
    </div>
  {:else}
    <!-- AUTHENTICATED DOSSIER VIEW -->

    <!-- ACTIVE DISPATCH & TRACKING CARD -->
    <VaultCard brackets={true} class="p-6" data-purpose="active-order-tracking">
      {#if activeOrder}
        <div class="flex flex-col sm:flex-row sm:items-center justify-between pb-4 border-b border-[#38332b] gap-3">
          <div>
            <div class="flex items-center space-x-2.5 flex-wrap gap-y-1">
              <span class="font-cinzel font-bold text-base text-[#f2e6cb] tracking-wider uppercase">
                Active Dispatch #{activeOrder.orderNumber}
              </span>
              <span class="text-[11px] font-mono text-[#8e897e]">
                dated {new Date(activeOrder.createdAt).toLocaleDateString('en-US', { month: 'short', day: 'numeric', year: 'numeric' })}
              </span>
              <span class="bg-amber-950/80 text-amber-300 border border-amber-600/60 text-[10px] px-2 py-0.5 font-cinzel font-semibold uppercase">
                {activeOrder.status}
              </span>
            </div>
            <p class="text-xs text-[#8e897e] mt-1 font-sans">
              Courier Service: <strong class="text-[#dfcaa0]">{activeOrder.customer.courierService || 'Imperial Courier'}</strong> •
              Tracking Cipher: <span class="font-mono text-vault-gold">IMP-{activeOrder.orderNumber.replace(/[^0-9A-Za-z]/g, '').slice(-8)}</span>
            </p>
          </div>
          <div class="flex items-center gap-3">
            <VaultButton
              variant="outline"
              size="sm"
              onclick={() => ui.notify(`Tracking parcel IMP-${activeOrder.orderNumber.replace(/[^0-9A-Za-z]/g, '').slice(-8)}: Scheduled transit to ${activeOrder.customer.city}`, 'gold')}
            >
              <svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path d="M10 6H6a2 2 0 00-2 2v10a2 2 0 002 2h10a2 2 0 002-2v-4M14 4h6m0 0v6m0-6L10 14" stroke-linecap="round" stroke-linejoin="round" stroke-width="2"></path>
              </svg>
              <span>Track Parcel</span>
            </VaultButton>
          </div>
        </div>

        <!-- Timeline Stepper -->
        <TimelineStepper currentStep={activeOrderStep} />

        <!-- Active Order Contents -->
        <div class="mt-8 pt-4 border-t border-[#25221c] flex flex-col md:flex-row items-start md:items-center justify-between gap-4 text-xs">
          <div class="flex items-center space-x-3 flex-wrap gap-y-2">
            <span class="text-[#8e897e] text-xs shrink-0">Consignment Items:</span>
            <div class="flex items-center space-x-2 flex-wrap gap-y-1">
              {#each activeOrder.items as item, idx}
                {#if idx > 0}
                  <span class="text-[#656054]">•</span>
                {/if}
                <div class="flex items-center space-x-1.5 bg-[#12141a] px-2 py-1 border border-[#2b2518]">
                  <div class="w-5 h-5 bg-[#181a24] border border-[#38332b] flex items-center justify-center text-[9px] text-vault-gold font-cinzel">
                    {item.miniatureName.slice(0, 2).toUpperCase()}
                  </div>
                  <span class="font-cinzel text-[#f2e6cb]">{item.miniatureName}</span>
                  <span class="font-mono text-gray-400">x{item.quantity}</span>
                </div>
              {/each}
            </div>
          </div>
          <div class="text-xs text-[#8e897e] shrink-0">
            Delivery Destination: <span class="text-[#dfcaa0]">{activeOrder.customer.city}, {activeOrder.customer.shippingAddress}</span>
          </div>
        </div>
      {:else}
        <!-- Empty Active Dispatch State -->
        <div class="text-center py-8">
          <div class="text-3xl text-vault-gold/60 mb-2">✠</div>
          <h3 class="font-cinzel text-sm font-bold text-[#f2e6cb] uppercase tracking-wider mb-1">
            No Active Dispatches in Transit
          </h3>
          <p class="text-xs text-[#8e897e] font-sans max-w-md mx-auto mb-4">
            All requisition mandates for {user.email} have been delivered, or no active deployments are currently underway.
          </p>
          <VaultButton
            variant="outline"
            size="sm"
            onclick={() => ui.navigateTo('catalog')}
          >
            Explore Munitorum Catalog
          </VaultButton>
        </div>
      {/if}
    </VaultCard>

    <!-- TWO COLUMN ROW: Commander Credentials + Loyalty Protocol -->
    <div class="grid grid-cols-1 lg:grid-cols-12 gap-8">
      <!-- COMMANDER CREDENTIALS CARD -->
      <VaultCard brackets={true} class="lg:col-span-5 p-6 flex flex-col justify-between" data-purpose="commander-profile-card">
        <div>
          <!-- Card Header -->
          <div class="flex items-center justify-between border-b border-[#38332b] pb-3 mb-5">
            <h2 class="font-cinzel font-bold text-sm tracking-[0.2em] text-vault-gold uppercase">Commander Credentials</h2>
            <span class="text-[10px] font-mono text-[#787265]">
              COMM-ID: #{user.email.split('@')[0].toUpperCase()}
            </span>
          </div>

          <!-- Heraldry Picker Section -->
          <div class="mb-5">
            <span class="block text-[10px] font-cinzel uppercase tracking-widest text-[#a7a296] mb-2.5">
              Favorite Faction Heraldry:
            </span>
            <div class="grid grid-cols-4 gap-2">
              <!-- 1. Imperium -->
              <button
                type="button"
                class="flex flex-col items-center justify-center p-2.5 transition-colors group relative cursor-pointer {user.heraldry === 'Imperium' ? 'bg-[#231c14] border-2 border-vault-gold text-vault-gold shadow-[0_0_8px_rgba(197,155,67,0.3)]' : 'bg-[#12141a] border border-[#38332b] text-[#8e897e] hover:text-vault-gold'}"
                onclick={() => user.setHeraldry('Imperium')}
                title="Imperium of Man"
              >
                <svg class="w-6 h-6 mb-1" fill="currentColor" viewBox="0 0 24 24">
                  <path d="M12 2L4 9l2 9 6 3 6-3 2-9-8-7zm0 3.5l5 4.5-1.5 6.5-3.5-1.7V10h-2v4.8L6.5 16.5 5 10l5-4.5z"></path>
                </svg>
                <span class="text-[9px] font-cinzel font-semibold uppercase tracking-wider">Imperium</span>
                {#if user.heraldry === 'Imperium'}
                  <span class="absolute -top-1 -right-1 w-2 h-2 bg-vault-gold rounded-full"></span>
                {/if}
              </button>

              <!-- 2. Chaos -->
              <button
                type="button"
                class="flex flex-col items-center justify-center p-2.5 transition-colors group relative cursor-pointer {user.heraldry === 'Chaos' ? 'bg-[#251010] border-2 border-red-600 text-red-400 shadow-[0_0_8px_rgba(255,100,100,0.3)]' : 'bg-[#12141a] border border-[#38332b] text-[#8e897e] hover:text-red-400'}"
                onclick={() => user.setHeraldry('Chaos')}
                title="Forces of Chaos"
              >
                <svg class="w-6 h-6 mb-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <circle cx="12" cy="12" r="3" stroke-width="2"></circle>
                  <path d="M12 2v4m0 12v4M2 12h4m12 0h4m-3.1-6.9l-2.8 2.8m-8.2 8.2l-2.8 2.8m0-13.8l2.8 2.8m8.2 8.2l2.8 2.8" stroke-width="2"></path>
                </svg>
                <span class="text-[9px] font-cinzel uppercase tracking-wider">Chaos</span>
                {#if user.heraldry === 'Chaos'}
                  <span class="absolute -top-1 -right-1 w-2 h-2 bg-red-500 rounded-full"></span>
                {/if}
              </button>

              <!-- 3. Necrons -->
              <button
                type="button"
                class="flex flex-col items-center justify-center p-2.5 transition-colors group relative cursor-pointer {user.heraldry === 'Necrons' ? 'bg-[#0f1f18] border-2 border-emerald-500 text-emerald-300 shadow-[0_0_8px_rgba(16,185,129,0.3)]' : 'bg-[#12141a] border border-[#38332b] text-[#8e897e] hover:text-emerald-400'}"
                onclick={() => user.setHeraldry('Necrons')}
                title="Necron Dynasty"
              >
                <svg class="w-6 h-6 mb-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <polygon points="12,2 4,8 7,18 12,22 17,18 20,8" stroke-width="2"></polygon>
                  <circle cx="12" cy="12" fill="currentColor" r="2.5"></circle>
                </svg>
                <span class="text-[9px] font-cinzel uppercase tracking-wider">Necrons</span>
                {#if user.heraldry === 'Necrons'}
                  <span class="absolute -top-1 -right-1 w-2 h-2 bg-emerald-500 rounded-full"></span>
                {/if}
              </button>

              <!-- 4. Orks -->
              <button
                type="button"
                class="flex flex-col items-center justify-center p-2.5 transition-colors group relative cursor-pointer {user.heraldry === 'Orks' ? 'bg-[#231c0f] border-2 border-amber-500 text-amber-300 shadow-[0_0_8px_rgba(245,158,11,0.3)]' : 'bg-[#12141a] border border-[#38332b] text-[#8e897e] hover:text-amber-400'}"
                onclick={() => user.setHeraldry('Orks')}
                title="Ork WAAAGH!"
              >
                <svg class="w-6 h-6 mb-1" fill="currentColor" viewBox="0 0 24 24">
                  <path d="M4 6h16v10H4zM6 9h3v4H6zm9 0h3v4h-3zM8 18l4 4 4-4H8z"></path>
                </svg>
                <span class="text-[9px] font-cinzel uppercase tracking-wider">Orks</span>
                {#if user.heraldry === 'Orks'}
                  <span class="absolute -top-1 -right-1 w-2 h-2 bg-amber-500 rounded-full"></span>
                {/if}
              </button>
            </div>

            <!-- Quick catalog filter link -->
            <VaultButton
              variant="outline"
              size="sm"
              class="mt-3 w-full text-[10px]"
              onclick={() => { ui.setSearch(user.heraldry); ui.navigateTo('catalog'); }}
            >
              Explore Sanctioned {user.heraldry} Manifest
            </VaultButton>
          </div>

          <!-- Personal Data Fields -->
          <CornerBrackets class="space-y-3 bg-[#0d0f15] p-4 border border-[#38332b] mb-5">
            <div>
              <span class="text-[10px] font-cinzel uppercase tracking-wider text-[#8e897e] block">Name / Callsign</span>
              <div class="font-cinzel font-bold text-base text-[#f2e6cb] mt-0.5">{user.name || 'Unnamed Commander'}</div>
            </div>
            <div class="pt-2 border-t border-[#25221c]">
              <span class="text-[10px] font-cinzel uppercase tracking-wider text-[#8e897e] block">Email Address</span>
              <div class="font-mono text-xs text-[#dfcaa0] mt-0.5">{user.email}</div>
            </div>
            <div class="pt-2 border-t border-[#25221c]">
              <span class="text-[10px] font-cinzel uppercase tracking-wider text-[#8e897e] block">Phone / Comm-line</span>
              <div class="font-mono text-xs text-vault-gold mt-0.5">{user.phone || 'Not configured'}</div>
            </div>
          </CornerBrackets>
        </div>

        <!-- Profile Action Buttons -->
        <div class="flex items-center space-x-3 pt-3 border-t border-[#38332b]">
          <VaultButton
            variant="outline"
            size="sm"
            class="flex-1"
            onclick={openEditProfile}
          >
            Update Profile
          </VaultButton>
          <VaultButton
            variant="crimson"
            size="sm"
            onclick={handleLogout}
          >
            <svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1" stroke-linecap="round" stroke-linejoin="round" stroke-width="2"></path>
            </svg>
            <span>Log Out</span>
          </VaultButton>
        </div>
      </VaultCard>

      <!-- MUNITORUM LOYALTY PROTOCOL CARD -->
      <VaultCard brackets={true} class="lg:col-span-7 p-6 flex flex-col justify-between" data-purpose="loyalty-program">
        <div>
          <!-- Header -->
          <div class="flex items-center justify-between border-b border-[#38332b] pb-3 mb-5">
            <div>
              <h2 class="font-cinzel font-bold text-sm tracking-[0.2em] text-vault-gold uppercase">Vault Loyalty Protocol</h2>
              <p class="text-xs text-[#8e897e] mt-0.5 font-sans">Accumulated tier discount & personal promo codes</p>
            </div>
            <span class="text-xs font-cinzel font-bold text-amber-300 bg-amber-950/60 border border-amber-600/50 px-2.5 py-1">
              Rank: {loyaltyTier.name} ({loyaltyTier.discount}%)
            </span>
          </div>

          <!-- Progress Indicator (USD) -->
          <CornerBrackets class="bg-[#0e1017] border border-[#38332b] p-4 mb-5">
            <div class="flex items-center justify-between text-xs mb-2">
              <span class="text-[#a7a296] font-sans">
                Expended <strong class="text-vault-gold font-mono font-bold">${totalSpent.toFixed(2)}</strong>
                {#if !loyaltyTier.isMax}
                  / ${loyaltyTier.nextThreshold} to Next Rank
                {:else}
                  (Maximum Rank Attained)
                {/if}
              </span>
              <span class="text-[11px] font-mono text-emerald-400">
                {loyaltyTier.progress}%
              </span>
            </div>
            <div class="w-full bg-[#1b1e28] h-2 relative overflow-hidden border border-[#38332b]">
              <div
                class="bg-gradient-to-r from-amber-600 to-vault-gold h-full shadow-[0_0_8px_rgba(197,155,67,0.7)]"
                style="width: {loyaltyTier.progress}%"
              ></div>
            </div>
            <div class="grid grid-cols-3 gap-2 text-center mt-3 pt-3 border-t border-[#25221c]">
              <!-- Rank 1: Scout -->
              <CornerBrackets class="p-2 {loyaltyTier.name === 'Scout' ? 'bg-[#251d14] border-2 border-vault-gold shadow-[0_0_10px_rgba(197,155,67,0.35)] relative' : 'bg-[#12141a] border border-[#2d2820]'}">
                {#if loyaltyTier.name === 'Scout'}
                  <span class="absolute -top-2 left-1/2 -translate-x-1/2 bg-vault-gold text-[#0a0b0e] text-[8px] font-cinzel font-bold uppercase px-1.5 py-0.2">
                    ACTIVE
                  </span>
                {/if}
                <div class="text-[10px] font-mono text-[#787265] uppercase">Novice Rank</div>
                <div class="font-cinzel text-xs font-semibold {loyaltyTier.name === 'Scout' ? 'text-amber-300 font-bold' : 'text-[#a7a296]'} mt-0.5">Scout (5%)</div>
                <div class="text-[9px] text-[#787265] font-mono mt-0.5">from $100</div>
              </CornerBrackets>

              <!-- Rank 2: Veteran -->
              <CornerBrackets class="p-2 {loyaltyTier.name === 'Veteran' ? 'bg-[#251d14] border-2 border-vault-gold shadow-[0_0_10px_rgba(197,155,67,0.35)] relative' : 'bg-[#12141a] border border-[#2d2820]'}">
                {#if loyaltyTier.name === 'Veteran'}
                  <span class="absolute -top-2 left-1/2 -translate-x-1/2 bg-vault-gold text-[#0a0b0e] text-[8px] font-cinzel font-bold uppercase px-1.5 py-0.2">
                    ACTIVE
                  </span>
                {/if}
                <div class="text-[10px] font-mono text-[#787265] uppercase">Veteran Rank</div>
                <div class="font-cinzel text-xs font-semibold {loyaltyTier.name === 'Veteran' ? 'text-amber-300 font-bold' : 'text-[#a7a296]'} mt-0.5">Veteran (7%)</div>
                <div class="text-[9px] text-[#787265] font-mono mt-0.5">from $300</div>
              </CornerBrackets>

              <!-- Rank 3: Master of the Forge -->
              <CornerBrackets class="p-2 {loyaltyTier.name === 'Master of the Forge' ? 'bg-[#251d14] border-2 border-vault-gold shadow-[0_0_10px_rgba(197,155,67,0.35)] relative' : 'bg-[#12141a] border border-[#2d2820]'}">
                {#if loyaltyTier.name === 'Master of the Forge'}
                  <span class="absolute -top-2 left-1/2 -translate-x-1/2 bg-vault-gold text-[#0a0b0e] text-[8px] font-cinzel font-bold uppercase px-1.5 py-0.2">
                    ACTIVE
                  </span>
                {/if}
                <div class="text-[10px] font-mono text-[#787265] uppercase">Supreme Tier</div>
                <div class="font-cinzel text-xs font-semibold {loyaltyTier.name === 'Master of the Forge' ? 'text-amber-300 font-bold' : 'text-[#a7a296]'} mt-0.5">Master of Forge (10%)</div>
                <div class="text-[9px] text-[#787265] font-mono mt-0.5">from $600</div>
              </CornerBrackets>
            </div>
          </CornerBrackets>

          <!-- Personal Promocodes List -->
          <div>
            <span class="block text-[10px] font-cinzel uppercase tracking-widest text-[#a7a296] mb-2.5">
              Sanctioned Munitorum Promo Codes:
            </span>
            <div class="space-y-2.5">
              <!-- Promo 1 -->
              <CornerBrackets class="flex items-center justify-between p-2.5 bg-[#0f1118] border border-[#38332b]">
                <div class="flex items-center space-x-3">
                  <span class="px-2 py-1 bg-[#231b12] border border-vault-gold/60 text-vault-gold font-mono font-bold text-xs tracking-wider">
                    WARHAMMER10
                  </span>
                  <div>
                    <div class="text-xs text-[#f2e6cb] font-sans">10% off Any Requisition Order</div>
                    <div class="text-[10px] text-[#787265] font-mono">Sanctioned for all Imperial citizens</div>
                  </div>
                </div>
                <VaultButton
                  variant="outline"
                  size="sm"
                  onclick={() => copyPromo('WARHAMMER10')}
                >
                  Copy Code
                </VaultButton>
              </CornerBrackets>

              <!-- Promo 2 -->
              <CornerBrackets class="flex items-center justify-between p-2.5 bg-[#0f1118] border border-[#38332b]">
                <div class="flex items-center space-x-3">
                  <span class="px-2 py-1 bg-[#231b12] border border-vault-gold/60 text-vault-gold font-mono font-bold text-xs tracking-wider">
                    TERRA10
                  </span>
                  <div>
                    <div class="text-xs text-[#f2e6cb] font-sans">10% Imperial Tithe Relief</div>
                    <div class="text-[10px] text-[#787265] font-mono">Valid for all Standard Dispatches</div>
                  </div>
                </div>
                <VaultButton
                  variant="outline"
                  size="sm"
                  onclick={() => copyPromo('TERRA10')}
                >
                  Copy Code
                </VaultButton>
              </CornerBrackets>

              <!-- Promo 3 -->
              <CornerBrackets class="flex items-center justify-between p-2.5 bg-[#0f1118] border border-[#38332b]">
                <div class="flex items-center space-x-3">
                  <span class="px-2 py-1 bg-[#231b12] border border-vault-gold/60 text-vault-gold font-mono font-bold text-xs tracking-wider">
                    EMPEROR20
                  </span>
                  <div>
                    <div class="text-xs text-[#f2e6cb] font-sans">20% Golden Throne Benediction</div>
                    <div class="text-[10px] text-[#787265] font-mono">Applies to Orders &gt; $100</div>
                  </div>
                </div>
                <VaultButton
                  variant="outline"
                  size="sm"
                  onclick={() => copyPromo('EMPEROR20')}
                >
                  Copy Code
                </VaultButton>
              </CornerBrackets>
            </div>
          </div>
        </div>
      </VaultCard>
    </div>

    <!-- ORDER HISTORY ARCHIVE -->
    <VaultCard brackets={true} class="p-6" data-purpose="order-history-ledger">
      <div class="flex flex-col sm:flex-row sm:items-center justify-between pb-4 border-b border-[#38332b] mb-5 gap-3">
        <div>
          <h2 class="font-cinzel font-bold text-base text-[#f2e6cb] tracking-wider uppercase">
            Consignment Archive & Order History
          </h2>
          <p class="text-xs text-[#8e897e] font-sans">Complete ledger of past orders, delivery statuses, and invoices from PostgreSQL</p>
        </div>
        <div class="flex items-center space-x-2">
          <label class="text-[11px] text-[#8e897e] font-cinzel uppercase" for="orderSortSelect">Sort by:</label>
          <select
            id="orderSortSelect"
            class="bg-[#101217] border border-[#5a482b] text-xs text-[#dfcaa0] font-cinzel py-1 px-3 focus:outline-none focus:border-vault-gold cursor-pointer"
            bind:value={orderSort}
          >
            <option value="newest">Date (Newest First)</option>
            <option value="delivered">Delivered Only</option>
            <option value="total">By Total Value</option>
          </select>
        </div>
      </div>

      <!-- Order Cards List -->
      <div class="space-y-4">
        {#if loadingOrders}
          <div class="text-center py-12 text-vault-gold font-cinzel text-xs">
            Querying Imperial Munitorum archives for {user.email}...
          </div>
        {:else if sortedOrders.length === 0}
          <div class="text-center py-12 border border-[#2a261e] bg-[#0c0e14] p-6">
            <div class="text-3xl text-vault-gold mb-2">✠</div>
            <h3 class="font-cinzel text-sm font-bold text-gray-300 uppercase tracking-wider mb-1">
              No Requisition Orders Found
            </h3>
            <p class="text-xs text-gray-500 font-sans mb-4">
              No past dispatches have been registered in the database for {user.email}.
            </p>
            <VaultButton
              variant="outline"
              size="sm"
              onclick={() => ui.navigateTo('catalog')}
            >
              Explore Munitorum Catalog
            </VaultButton>
          </div>
        {:else}
          {#each sortedOrders as ord (ord.id)}
            <CornerBrackets class="border border-[#38332b] bg-[#0f1117] p-4">
              <div class="flex flex-col sm:flex-row sm:items-center justify-between pb-3 border-b border-[#25221c] gap-2">
                <div class="flex flex-wrap items-center gap-3">
                  <span class="font-cinzel font-bold text-sm text-vault-gold tracking-wide">
                    ORDER #{ord.orderNumber}
                  </span>
                  <span class="text-[11px] font-mono text-[#787265]">
                    {new Date(ord.createdAt).toLocaleDateString()}
                  </span>
                  <span class="text-[10px] px-2.5 py-0.5 font-cinzel uppercase font-semibold {ord.status.toLowerCase().includes('delivered') ? 'bg-emerald-950/70 text-emerald-300 border border-emerald-600/50' : ord.status.toLowerCase().includes('canceled') ? 'bg-[#1f2128] text-[#8e897e] border border-[#3d4251]' : 'bg-amber-950/70 text-amber-300 border border-amber-600/50'}">
                    {ord.status}
                  </span>
                </div>
                <div class="text-right">
                  <span class="font-cinzel font-bold text-base text-[#f2e6cb]">
                    ${ord.totalAmount.toFixed(2)}
                  </span>
                </div>
              </div>

              <div class="py-3.5 flex flex-col md:flex-row md:items-center justify-between gap-4">
                <!-- Items preview -->
                <div class="flex items-center space-x-3.5">
                  <div class="w-12 h-12 bg-[#171922] border border-[#38332b] flex items-center justify-center shrink-0 p-1">
                    <svg class="w-7 h-7 text-vault-gold" fill="currentColor" viewBox="0 0 24 24">
                      <path d="M12 2l3 5 5 1-4 4 1 5-5-3-5 3 1-5-4-4 5-1z"></path>
                    </svg>
                  </div>
                  <div>
                    <h4 class="font-cinzel font-semibold text-xs text-[#dfcaa0]">
                      {ord.items.map(i => `${i.miniatureName} (x${i.quantity})`).join(', ')}
                    </h4>
                    <p class="text-[11px] text-[#787265] mt-0.5 font-sans">
                      {ord.items.length} Units total • Dispatch via {ord.customer.courierService || 'Imperial Relay'}
                    </p>
                  </div>
                </div>

                <!-- Buttons -->
                <div class="flex items-center space-x-2 shrink-0">
                  <VaultButton
                    variant="dark"
                    size="sm"
                    onclick={() => ui.notify(`Order #${ord.orderNumber}: Destination ${ord.customer.shippingAddress}, ${ord.customer.city} (${ord.customer.courierService})`, 'gold')}
                  >
                    Details
                  </VaultButton>
                  <VaultButton
                    variant="gold"
                    size="sm"
                    onclick={() => handleRepeatOrder(ord)}
                  >
                    Repeat Order
                  </VaultButton>
                  <VaultButton
                    variant="outline"
                    size="sm"
                    title="Download Invoice"
                    onclick={() => ui.notify(`Munitorum requisition invoice for #${ord.orderNumber} ($${ord.totalAmount.toFixed(2)}) rendered.`, 'emerald')}
                  >
                    <svg class="w-4 h-4 inline-block -mt-0.5 mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" stroke-linecap="round" stroke-linejoin="round" stroke-width="2"></path>
                    </svg>
                    <span>Invoice</span>
                  </VaultButton>
                </div>
              </div>
            </CornerBrackets>
          {/each}
        {/if}
      </div>
    </VaultCard>

    <!-- SAVED DELIVERY ADDRESSES -->
    <VaultCard brackets={true} class="p-6" data-purpose="saved-delivery-addresses">
      <div class="flex flex-col sm:flex-row sm:items-center justify-between pb-4 border-b border-[#38332b] mb-5 gap-3">
        <div>
          <h2 class="font-cinzel font-bold text-base text-[#f2e6cb] tracking-wider uppercase">
            Saved Delivery Addresses
          </h2>
          <p class="text-xs text-[#8e897e] font-sans">Registered pick-up points (PVZ) and courier addresses for rapid checkout</p>
        </div>
        <VaultButton
          variant="gold"
          size="sm"
          onclick={() => { newAddressModalOpen = true; }}
        >
          <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path d="M12 4v16m8-8H4" stroke-linecap="round" stroke-linejoin="round" stroke-width="2"></path>
          </svg>
          <span>+ Add New Address</span>
        </VaultButton>
      </div>

      <!-- Grid of Saved Addresses -->
      {#if user.addresses.length === 0}
        <div class="text-center py-6 text-xs text-gray-500 font-sans border border-[#2a261e] bg-[#0c0e14]">
          No saved delivery addresses registered yet. Click "+ Add New Address" to register shipping coordinates.
        </div>
      {:else}
        <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
          {#each user.addresses as addr (addr.id)}
            <CornerBrackets class="p-4 {addr.isPrimary ? 'bg-[#141822] border-2 border-vault-gold shadow-[0_0_10px_rgba(197,155,67,0.25)]' : 'bg-[#0f1118] border border-[#38332b]'} relative flex flex-col justify-between">
              <div>
                <div class="flex items-center justify-between mb-2">
                  {#if addr.isPrimary}
                    <span class="text-[9px] font-cinzel uppercase bg-vault-gold text-black px-2 py-0.5 font-bold tracking-wider">
                      Default Address
                    </span>
                  {:else}
                    <span class="text-[9px] font-cinzel uppercase bg-[#1f2129] text-[#a7a296] px-2 py-0.5 font-bold tracking-wider">
                      {addr.type}
                    </span>
                  {/if}
                  <span class="text-xs text-vault-gold font-mono font-bold">
                    {addr.type === 'PVZ' ? 'CDEK PVZ' : addr.type === 'Courier' ? 'Direct Delivery' : 'Postal Relay'}
                  </span>
                </div>
                <h4 class="font-cinzel font-bold text-sm text-[#f2e6cb]">{addr.city}</h4>
                <p class="text-xs text-[#dfcaa0] mt-1 font-sans">
                  {addr.line1}
                </p>
                <div class="text-[11px] text-[#8e897e] mt-2 font-mono">
                  {addr.details}
                </div>
              </div>

              <div class="mt-4 pt-3 border-t border-[#38332b] flex items-center justify-between">
                <label class="flex items-center space-x-2 text-[11px] text-vault-gold cursor-pointer">
                  <input
                    type="radio"
                    name="default_addr"
                    checked={addr.isPrimary}
                    class="text-vault-gold focus:ring-0 bg-[#0c0d12] border-[#5a482b]"
                    onchange={() => user.setPrimaryAddress(addr.id)}
                  />
                  <span class="font-cinzel">{addr.isPrimary ? 'Primary' : 'Set as Primary'}</span>
                </label>
                <button
                  type="button"
                  class="text-xs text-red-400 hover:text-red-300 transition-colors font-cinzel uppercase cursor-pointer"
                  title="Delete Address"
                  onclick={() => user.deleteAddress(addr.id)}
                >
                  Delete
                </button>
              </div>
            </CornerBrackets>
          {/each}
        </div>
      {/if}
    </VaultCard>
  {/if}

  <!-- Modal Dialog for Registering Delivery Coordinates -->
  <VaultModal
    open={newAddressModalOpen}
    title="Register New Imperial Delivery Coordinates"
    maxWidth="max-w-2xl"
    onclose={() => { newAddressModalOpen = false; }}
  >
    <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
      <div>
        <label class="block text-[11px] font-cinzel text-gray-300 uppercase mb-1" for="newCityInput">City / Hive Sector</label>
        <input
          id="newCityInput"
          class="w-full bg-[#0e1017] border border-[#5a482b] text-xs p-2 text-white focus:border-vault-gold focus:outline-none"
          placeholder="e.g. Moscow"
          bind:value={newCity}
        />
      </div>
      <div>
        <label class="block text-[11px] font-cinzel text-gray-300 uppercase mb-1" for="newLine1Input">Address / Hub Code</label>
        <input
          id="newLine1Input"
          class="w-full bg-[#0e1017] border border-[#5a482b] text-xs p-2 text-white focus:border-vault-gold focus:outline-none"
          placeholder="e.g. PVZ #204, Prospekt Mira 14"
          bind:value={newLine1}
        />
      </div>
      <div>
        <label class="block text-[11px] font-cinzel text-gray-300 uppercase mb-1" for="newTypeSelect">Service Type</label>
        <select
          id="newTypeSelect"
          class="w-full bg-[#0e1017] border border-[#5a482b] text-xs p-2 text-white focus:border-vault-gold focus:outline-none cursor-pointer"
          bind:value={newType}
        >
          <option value="PVZ">CDEK PVZ (Self Pickup)</option>
          <option value="Courier">Courier Doorstep Delivery</option>
          <option value="Postal">Russian Post Relay</option>
        </select>
      </div>
    </div>
    <div class="flex justify-end gap-3 mt-6 pt-3 border-t border-[#38332b]">
      <VaultButton variant="outline" size="sm" onclick={() => { newAddressModalOpen = false; }}>
        Cancel
      </VaultButton>
      <VaultButton variant="gold" size="sm" onclick={handleAddAddress}>
        Save Address
      </VaultButton>
    </div>
  </VaultModal>

  <!-- Modal Dialog for Updating Profile -->
  <VaultModal
    open={editProfileModalOpen}
    title="Update Commander Credentials"
    maxWidth="max-w-md"
    onclose={() => { editProfileModalOpen = false; }}
  >
    <div class="space-y-4">
      <div>
        <label class="block text-[11px] font-cinzel text-gray-300 uppercase mb-1" for="editNameInput">
          Name / Callsign
        </label>
        <input
          id="editNameInput"
          class="w-full bg-[#0e1017] border border-[#5a482b] text-xs p-2 text-white focus:border-vault-gold focus:outline-none"
          bind:value={editName}
        />
      </div>
      <div>
        <label class="block text-[11px] font-cinzel text-gray-300 uppercase mb-1" for="editPhoneInput">
          Phone / Comm-line
        </label>
        <input
          id="editPhoneInput"
          class="w-full bg-[#0e1017] border border-[#5a482b] text-xs p-2 text-white focus:border-vault-gold focus:outline-none font-mono"
          bind:value={editPhone}
        />
      </div>
    </div>
    <div class="flex justify-end gap-3 mt-6 pt-3 border-t border-[#38332b]">
      <VaultButton variant="outline" size="sm" onclick={() => { editProfileModalOpen = false; }}>
        Cancel
      </VaultButton>
      <VaultButton variant="gold" size="sm" onclick={handleSaveProfile}>
        Save Credentials
      </VaultButton>
    </div>
  </VaultModal>
</main>
