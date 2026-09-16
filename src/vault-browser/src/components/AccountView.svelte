<script lang="ts">
  import { onMount } from 'svelte';
  import { user } from '../lib/state/user.svelte';
  import { cart } from '../lib/state/cart.svelte';
  import { ui } from '../lib/state/ui.svelte';
  import { fetchUserOrders } from '../lib/api';
  import type { Order } from '../types';
  import TimelineStepper from './ui/TimelineStepper.svelte';
  import CornerBrackets from './ui/CornerBrackets.svelte';

  let orders = $state<Order[]>([]);
  let orderSort = $state<'newest' | 'delivered' | 'total'>('newest');
  let newAddressModalOpen = $state(false);
  let newCity = $state('');
  let newLine1 = $state('');
  let newDetails = $state('');
  let newType = $state<'PVZ' | 'Courier' | 'Postal'>('PVZ');

  onMount(async () => {
    try {
      orders = await fetchUserOrders(user.email);
    } catch {
      // ignore
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
    ui.notify(`Requisition order ${order.orderNumber} items added to active cart!`, 'gold');
    ui.openCart();
  }

  function handleAddAddress() {
    if (!newCity || !newLine1) {
      ui.notify('City and address lines are required.', 'crimson');
      return;
    }
    user.addAddress({
      city: newCity,
      line1: newLine1,
      details: newDetails || 'Standard Imperial Relay',
      type: newType,
      isPrimary: false
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
      <button class="hover:text-vault-gold transition-colors" onclick={() => ui.navigateTo('catalog')}>Home</button>
      <span>/</span>
      <button class="hover:text-vault-gold transition-colors" onclick={() => ui.navigateTo('catalog')}>Catalog</button>
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
      <div class="flex items-center space-x-2 self-start md:self-auto bg-[#14151a] px-3 py-1.5 border border-[#38332b]">
        <span class="w-2 h-2 rounded-full bg-emerald-500"></span>
        <span class="font-cinzel text-[11px] uppercase tracking-widest text-[#a7a296]">Account Active</span>
      </div>
    </div>
  </div>
</section>
<!-- END: BreadcrumbAndBanner -->

<!-- MAIN CONTENT WRAPPER -->
<main class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8 flex-1 w-full space-y-8" data-purpose="user-dashboard">
  <!-- ACTIVE DISPATCH & TRACKING CARD (With 4 Golden L-Brackets) -->
  <section class="vault-card corner-brackets p-6" data-purpose="active-order-tracking">
    <span class="bracket-tl" aria-hidden="true"></span>
    <span class="bracket-tr" aria-hidden="true"></span>
    <span class="bracket-bl" aria-hidden="true"></span>
    <span class="bracket-br" aria-hidden="true"></span>

    <div class="flex flex-col sm:flex-row sm:items-center justify-between pb-4 border-b border-[#38332b] gap-3">
      <div>
        <div class="flex items-center space-x-2.5">
          <span class="font-cinzel font-bold text-base text-[#f2e6cb] tracking-wider uppercase">
            Active Dispatch #{user.activeOrderNumber}
          </span>
          <span class="text-[11px] font-mono text-[#8e897e]">dated {user.activeOrderDate}</span>
          <span class="bg-amber-950/80 text-amber-300 border border-amber-600/60 text-[10px] px-2 py-0.5 font-cinzel font-semibold uppercase">
            In Transit
          </span>
        </div>
        <p class="text-xs text-[#8e897e] mt-1 font-sans">
          Courier Service: <strong class="text-[#dfcaa0]">{user.activeCourier}</strong> • Tracking Cipher: <span class="font-mono text-vault-gold">{user.activeTrackingCipher}</span>
        </p>
      </div>
      <div class="flex items-center gap-3">
        <button
          class="btn-vault px-3.5 py-1.5 bg-[#231b12] hover:bg-[#342717] border border-vault-gold text-vault-gold text-xs font-cinzel uppercase tracking-wider flex items-center space-x-1.5"
          onclick={() => ui.notify(`Tracking parcel ${user.activeTrackingCipher}: Expected arrival at CDEK dispatch point in 48 hours.`, 'gold')}
        >
          <svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path d="M10 6H6a2 2 0 00-2 2v10a2 2 0 002 2h10a2 2 0 002-2v-4M14 4h6m0 0v6m0-6L10 14" stroke-linecap="round" stroke-linejoin="round" stroke-width="2"></path>
          </svg>
          <span>Track Parcel</span>
        </button>
      </div>
    </div>

    <!-- Timeline Stepper -->
    <TimelineStepper currentStep={user.activeStep} />

    <!-- Active Order Contents -->
    <div class="mt-8 pt-4 border-t border-[#25221c] flex flex-col md:flex-row items-start md:items-center justify-between gap-4 text-xs">
      <div class="flex items-center space-x-3">
        <span class="text-[#8e897e] text-xs">Consignment Items:</span>
        <div class="flex items-center space-x-2">
          <div class="w-8 h-8 bg-[#181a24] border border-[#38332b] flex items-center justify-center text-[10px] text-vault-gold font-cinzel">
            SM
          </div>
          <span class="font-cinzel text-[#f2e6cb]">Space Marine Captain</span>
          <span class="text-[#656054]">•</span>
          <div class="w-8 h-8 bg-[#181a24] border border-[#38332b] flex items-center justify-center text-[10px] text-vault-gold font-cinzel">
            CP
          </div>
          <span class="font-cinzel text-[#f2e6cb]">Citadel Base Paint Set</span>
        </div>
      </div>
      <div class="text-xs text-[#8e897e]">
        Delivery Destination: <span class="text-[#dfcaa0]">{user.activeDestination}</span>
      </div>
    </div>
  </section>

  <!-- TWO COLUMN ROW: Commander Credentials + Loyalty Protocol -->
  <div class="grid grid-cols-1 lg:grid-cols-12 gap-8">
    <!-- COMMANDER CREDENTIALS CARD (With 4 Golden L-Brackets) -->
    <section class="lg:col-span-5 vault-card corner-brackets p-6 flex flex-col justify-between" data-purpose="commander-profile-card">
      <span class="bracket-tl" aria-hidden="true"></span>
      <span class="bracket-tr" aria-hidden="true"></span>
      <span class="bracket-bl" aria-hidden="true"></span>
      <span class="bracket-br" aria-hidden="true"></span>

      <div>
        <div class="flex items-center justify-between border-b border-[#38332b] pb-3 mb-5">
          <h2 class="font-cinzel font-bold text-sm tracking-[0.2em] text-vault-gold uppercase">Commander Credentials</h2>
          <span class="text-[10px] font-mono text-[#787265]">USER ID: #USR-9482</span>
        </div>

        <!-- Heraldry Picker Section -->
        <div class="mb-5">
          <span class="block text-[10px] font-cinzel uppercase tracking-widest text-[#a7a296] mb-2.5">
            Favorite Faction Heraldry:
          </span>
          <div class="grid grid-cols-4 gap-2">
            <!-- 1. Imperium -->
            <button
              class="flex flex-col items-center justify-center p-2.5 transition-colors group relative {user.heraldry === 'Imperium' ? 'bg-[#231c14] border-2 border-vault-gold text-vault-gold shadow-[0_0_8px_rgba(197,155,67,0.3)]' : 'bg-[#12141a] border border-[#38332b] text-[#8e897e] hover:text-vault-gold'}"
              onclick={() => user.setHeraldry('Imperium')}
              title="Adeptus Astartes (Imperium)"
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
              class="flex flex-col items-center justify-center p-2.5 transition-colors group relative {user.heraldry === 'Chaos' ? 'bg-[#251010] border-2 border-red-600 text-red-400 shadow-[0_0_8px_rgba(255,100,100,0.3)]' : 'bg-[#12141a] border border-[#38332b] text-[#8e897e] hover:text-red-400'}"
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
              class="flex flex-col items-center justify-center p-2.5 transition-colors group relative {user.heraldry === 'Necrons' ? 'bg-[#0f1f18] border-2 border-emerald-500 text-emerald-300 shadow-[0_0_8px_rgba(16,185,129,0.3)]' : 'bg-[#12141a] border border-[#38332b] text-[#8e897e] hover:text-emerald-400'}"
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
              class="flex flex-col items-center justify-center p-2.5 transition-colors group relative {user.heraldry === 'Orks' ? 'bg-[#231c0f] border-2 border-amber-500 text-amber-300 shadow-[0_0_8px_rgba(245,158,11,0.3)]' : 'bg-[#12141a] border border-[#38332b] text-[#8e897e] hover:text-amber-400'}"
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
        </div>

        <!-- Personal Data Fields (With 4 Golden L-Brackets) -->
        <CornerBrackets class="space-y-3 bg-[#0d0f15] p-4 border border-[#38332b] mb-5">
          <div>
            <span class="text-[10px] font-cinzel uppercase tracking-wider text-[#8e897e] block">Name / Callsign</span>
            <div class="font-cinzel font-bold text-base text-[#f2e6cb] mt-0.5">{user.name}</div>
          </div>
          <div class="pt-2 border-t border-[#25221c]">
            <span class="text-[10px] font-cinzel uppercase tracking-wider text-[#8e897e] block">Email Address</span>
            <div class="font-mono text-xs text-[#dfcaa0] mt-0.5">{user.email}</div>
          </div>
          <div class="pt-2 border-t border-[#25221c]">
            <span class="text-[10px] font-cinzel uppercase tracking-wider text-[#8e897e] block">Phone / Comm-line</span>
            <div class="font-mono text-xs text-vault-gold mt-0.5">{user.phone}</div>
          </div>
        </CornerBrackets>
      </div>

      <!-- Profile Action Buttons -->
      <div class="flex items-center space-x-3 pt-3 border-t border-[#38332b]">
        <button
          class="btn-vault flex-1 py-2 px-3 bg-[#1d1e27] hover:bg-[#282a36] border border-[#5a482b] text-[#dfcaa0] hover:text-white text-xs font-cinzel uppercase tracking-wider"
          onclick={() => ui.notify('Security protocol cipher sent to inquisitor.malcor@imperium.vault', 'gold')}
        >
          Change Password
        </button>
        <button
          class="btn-vault py-2 px-3 bg-[#1e1313] hover:bg-red-950/60 border border-[#7f1d1d] text-red-400 hover:text-red-300 text-xs font-cinzel uppercase tracking-wider flex items-center space-x-1"
          onclick={() => ui.notify('Comm-link terminated. Re-authentication will be required.', 'crimson')}
        >
          <svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1" stroke-linecap="round" stroke-linejoin="round" stroke-width="2"></path>
          </svg>
          <span>Log Out</span>
        </button>
      </div>
    </section>

    <!-- MUNITORUM LOYALTY PROTOCOL CARD (With 4 Golden L-Brackets) -->
    <section class="lg:col-span-7 vault-card corner-brackets p-6 flex flex-col justify-between" data-purpose="loyalty-program">
      <span class="bracket-tl" aria-hidden="true"></span>
      <span class="bracket-tr" aria-hidden="true"></span>
      <span class="bracket-bl" aria-hidden="true"></span>
      <span class="bracket-br" aria-hidden="true"></span>

      <div>
        <!-- Header -->
        <div class="flex items-center justify-between border-b border-[#38332b] pb-3 mb-5">
          <div>
            <h2 class="font-cinzel font-bold text-sm tracking-[0.2em] text-vault-gold uppercase">Vault Loyalty Protocol</h2>
            <p class="text-xs text-[#8e897e] mt-0.5 font-sans">Accumulated tier discount & personal promo codes</p>
          </div>
          <span class="text-xs font-cinzel font-bold text-amber-300 bg-amber-950/60 border border-amber-600/50 px-2.5 py-1">
            Rank: {user.currentRankName}
          </span>
        </div>

        <!-- Progress Indicator -->
        <CornerBrackets class="bg-[#0e1017] border border-[#38332b] p-4 mb-5">
          <div class="flex items-center justify-between text-xs mb-2">
            <span class="text-[#a7a296] font-sans">
              Expended <strong class="text-vault-gold font-mono font-bold">{user.expendedRubles.toLocaleString()} ₽</strong> / {user.nextRankThreshold.toLocaleString()} ₽ to Next Rank
            </span>
            <span class="text-[11px] font-mono text-emerald-400">
              Remaining {(user.nextRankThreshold - user.expendedRubles).toLocaleString()} ₽
            </span>
          </div>

          <!-- Progress Bar -->
          <div class="w-full bg-[#1c1d26] h-2.5 border border-[#38332b] overflow-hidden p-0.5">
            <div
              class="bg-gradient-to-r from-vault-gold to-amber-400 h-full shadow-[0_0_8px_rgba(197,155,67,0.7)]"
              style="width: 69%;"
            ></div>
          </div>

          <!-- Rank Stepper -->
          <div class="grid grid-cols-3 gap-2 mt-4 pt-3 border-t border-[#25221c] text-center">
            <!-- Rank 1: Recruit -->
            <CornerBrackets class="p-2 bg-[#12141a] border border-[#2d2820]">
              <div class="text-[10px] font-mono text-[#787265] uppercase">Base Tier</div>
              <div class="font-cinzel text-xs font-semibold text-[#8e897e] mt-0.5">Recruit (3%)</div>
              <div class="text-[9px] text-[#5c574c] font-mono mt-0.5">up to 20,000 ₽</div>
            </CornerBrackets>

            <!-- Rank 2: Veteran (Active) -->
            <CornerBrackets class="p-2 bg-[#251d14] border-2 border-vault-gold shadow-[0_0_10px_rgba(197,155,67,0.35)] relative">
              <span class="absolute -top-2 left-1/2 -translate-x-1/2 bg-vault-gold text-[#0a0b0e] text-[8px] font-cinzel font-bold uppercase px-1.5 py-0.2">
                ACTIVE
              </span>
              <div class="text-[10px] font-mono text-vault-gold uppercase">Current Rank</div>
              <div class="font-cinzel text-xs font-bold text-amber-300 mt-0.5">Veteran (7%)</div>
              <div class="text-[9px] text-vault-gold/90 font-mono mt-0.5">from 20,000 ₽</div>
            </CornerBrackets>

            <!-- Rank 3: Master of the Forge -->
            <CornerBrackets class="p-2 bg-[#12141a] border border-[#2d2820]">
              <div class="text-[10px] font-mono text-[#787265] uppercase">Supreme Tier</div>
              <div class="font-cinzel text-xs font-semibold text-[#a7a296] mt-0.5">Master of Forge (10%)</div>
              <div class="text-[9px] text-[#787265] font-mono mt-0.5">from 50,000 ₽</div>
            </CornerBrackets>
          </div>
        </CornerBrackets>

        <!-- Personal Promocodes List -->
        <div>
          <span class="block text-[10px] font-cinzel uppercase tracking-widest text-[#a7a296] mb-2.5">
            Personal Promo Codes:
          </span>
          <div class="space-y-2.5">
            <!-- Promo 1 -->
            <CornerBrackets class="flex items-center justify-between p-2.5 bg-[#0f1118] border border-[#38332b]">
              <div class="flex items-center space-x-3">
                <span class="px-2 py-1 bg-[#231b12] border border-vault-gold/60 text-vault-gold font-mono font-bold text-xs tracking-wider">
                  WARP-TITHE-10
                </span>
                <div>
                  <div class="text-xs text-[#f2e6cb] font-sans">10% off Citadel Paints</div>
                  <div class="text-[10px] text-[#787265] font-mono">Valid until Nov 30, 2026</div>
                </div>
              </div>
              <button
                class="btn-vault px-3 py-1 bg-[#171922] hover:bg-[#25221a] border border-[#5a482b] text-vault-gold text-[10px] font-cinzel uppercase tracking-wider"
                onclick={() => copyPromo('WARP-TITHE-10')}
              >
                Copy Code
              </button>
            </CornerBrackets>

            <!-- Promo 2 -->
            <CornerBrackets class="flex items-center justify-between p-2.5 bg-[#0f1118] border border-[#38332b]">
              <div class="flex items-center space-x-3">
                <span class="px-2 py-1 bg-[#231b12] border border-vault-gold/60 text-vault-gold font-mono font-bold text-xs tracking-wider">
                  FORGE-DISPATCH
                </span>
                <div>
                  <div class="text-xs text-[#f2e6cb] font-sans">Free courier delivery</div>
                  <div class="text-[10px] text-[#787265] font-mono">Valid until Oct 15, 2026</div>
                </div>
              </div>
              <button
                class="btn-vault px-3 py-1 bg-[#171922] hover:bg-[#25221a] border border-[#5a482b] text-vault-gold text-[10px] font-cinzel uppercase tracking-wider"
                onclick={() => copyPromo('FORGE-DISPATCH')}
              >
                Copy Code
              </button>
            </CornerBrackets>

            <!-- Promo 3 -->
            <CornerBrackets class="flex items-center justify-between p-2.5 bg-[#0f1118] border border-[#38332b]">
              <div class="flex items-center space-x-3">
                <span class="px-2 py-1 bg-[#231b12] border border-vault-gold/60 text-vault-gold font-mono font-bold text-xs tracking-wider">
                  PRIMARIS-GIFT
                </span>
                <div>
                  <div class="text-xs text-[#f2e6cb] font-sans">500 ₽ off Infantry squads</div>
                  <div class="text-[10px] text-[#787265] font-mono">Valid until Dec 01, 2026</div>
                </div>
              </div>
              <button
                class="btn-vault px-3 py-1 bg-[#171922] hover:bg-[#25221a] border border-[#5a482b] text-vault-gold text-[10px] font-cinzel uppercase tracking-wider"
                onclick={() => copyPromo('PRIMARIS-GIFT')}
              >
                Copy Code
              </button>
            </CornerBrackets>
          </div>
        </div>
      </div>
    </section>
  </div>

  <!-- ORDER HISTORY ARCHIVES CARD (With 4 Golden L-Brackets) -->
  <section class="vault-card corner-brackets p-6" data-purpose="orders-archive">
    <span class="bracket-tl" aria-hidden="true"></span>
    <span class="bracket-tr" aria-hidden="true"></span>
    <span class="bracket-bl" aria-hidden="true"></span>
    <span class="bracket-br" aria-hidden="true"></span>

    <div class="flex flex-col sm:flex-row sm:items-center justify-between pb-4 border-b border-[#38332b] mb-5 gap-3">
      <div>
        <h2 class="font-cinzel font-bold text-base text-[#f2e6cb] tracking-wider uppercase">
          Order History & Archive
        </h2>
        <p class="text-xs text-[#8e897e] font-sans">Complete ledger of past orders, delivery statuses, and invoices</p>
      </div>
      <div class="flex items-center space-x-2">
        <label class="text-[11px] text-[#8e897e] font-cinzel uppercase" for="orderSortSelect">Sort by:</label>
        <select
          id="orderSortSelect"
          class="bg-[#101217] border border-[#5a482b] text-xs text-[#dfcaa0] font-cinzel py-1 px-3 focus:outline-none focus:border-vault-gold"
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
              <button
                class="btn-vault px-3 py-1.5 bg-[#161822] hover:bg-[#25221c] border border-[#4a3e28] text-xs font-cinzel uppercase text-[#c59b43]"
                onclick={() => ui.notify(`Manifest for order #${ord.orderNumber}: Destination ${ord.customer.shippingAddress}, ${ord.customer.city}`, 'gold')}
              >
                Details
              </button>
              <button
                class="btn-vault px-3 py-1.5 bg-[#251d14] hover:bg-vault-gold hover:text-black border border-vault-gold text-xs font-cinzel font-semibold uppercase text-vault-gold transition-colors"
                onclick={() => handleRepeatOrder(ord)}
              >
                Repeat Order
              </button>
              <button
                class="btn-vault px-2.5 py-1.5 bg-[#161822] hover:bg-[#25221c] border border-[#4a3e28] text-xs font-cinzel text-[#a7a296] hover:text-[#dfcaa0]"
                title="Download Invoice (PDF)"
                onclick={() => ui.notify(`Inquisitorial invoice PDF for #${ord.orderNumber} generated.`, 'emerald')}
              >
                <svg class="w-4 h-4 inline-block -mt-0.5 mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" stroke-linecap="round" stroke-linejoin="round" stroke-width="2"></path>
                </svg>
                <span>PDF</span>
              </button>
            </div>
          </div>
        </CornerBrackets>
      {/each}
    </div>
  </section>

  <!-- SAVED DELIVERY ADDRESSES (With 4 Golden L-Brackets on All Cards) -->
  <section class="vault-card corner-brackets p-6" data-purpose="saved-delivery-addresses">
    <span class="bracket-tl" aria-hidden="true"></span>
    <span class="bracket-tr" aria-hidden="true"></span>
    <span class="bracket-bl" aria-hidden="true"></span>
    <span class="bracket-br" aria-hidden="true"></span>

    <div class="flex flex-col sm:flex-row sm:items-center justify-between pb-4 border-b border-[#38332b] mb-5 gap-3">
      <div>
        <h2 class="font-cinzel font-bold text-base text-[#f2e6cb] tracking-wider uppercase">
          Saved Delivery Addresses
        </h2>
        <p class="text-xs text-[#8e897e] font-sans">Registered pick-up points (PVZ) and courier addresses for rapid checkout</p>
      </div>
      <button
        class="btn-vault px-3.5 py-2 bg-[#251d14] hover:bg-vault-gold hover:text-[#0a0b0e] border border-vault-gold text-vault-gold text-xs font-cinzel uppercase font-semibold tracking-wider flex items-center space-x-1.5 self-start sm:self-auto transition-colors"
        onclick={() => { newAddressModalOpen = true; }}
      >
        <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path d="M12 4v16m8-8H4" stroke-linecap="round" stroke-linejoin="round" stroke-width="2"></path>
        </svg>
        <span>+ Add New Address</span>
      </button>
    </div>

    <!-- Grid of Saved Addresses -->
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
            <div class="flex items-center space-x-2">
              <button
                class="text-xs text-red-400 hover:text-red-300 transition-colors font-cinzel uppercase"
                title="Delete Address"
                onclick={() => user.deleteAddress(addr.id)}
              >
                Delete
              </button>
            </div>
          </div>
        </CornerBrackets>
      {/each}
    </div>
  </section>

  <!-- Inline New Address Dialog -->
  {#if newAddressModalOpen}
    <CornerBrackets class="vault-card p-6 border-2 border-vault-gold">
      <div class="flex items-center justify-between pb-3 border-b border-[#38332b] mb-4">
        <h3 class="font-cinzel font-bold text-sm text-vault-brightGold uppercase tracking-wider">
          Register New Imperial Delivery Coordinates
        </h3>
        <button class="text-gray-400 hover:text-white" onclick={() => { newAddressModalOpen = false; }}>✕</button>
      </div>
      <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
        <div>
          <label class="block text-[11px] font-cinzel text-gray-300 uppercase mb-1" for="newCityInput">City / Hive Sector</label>
          <input
            id="newCityInput"
            class="w-full bg-[#0e1017] border border-[#5a482b] text-xs p-2 text-white focus:border-vault-gold"
            placeholder="e.g. Moscow / Sector Prime"
            bind:value={newCity}
          />
        </div>
        <div>
          <label class="block text-[11px] font-cinzel text-gray-300 uppercase mb-1" for="newLine1Input">Address / Hub Code</label>
          <input
            id="newLine1Input"
            class="w-full bg-[#0e1017] border border-[#5a482b] text-xs p-2 text-white focus:border-vault-gold"
            placeholder="e.g. PVZ #204, Prospekt Mira 14"
            bind:value={newLine1}
          />
        </div>
        <div>
          <label class="block text-[11px] font-cinzel text-gray-300 uppercase mb-1" for="newTypeSelect">Service Type</label>
          <select
            id="newTypeSelect"
            class="w-full bg-[#0e1017] border border-[#5a482b] text-xs p-2 text-white focus:border-vault-gold"
            bind:value={newType}
          >
            <option value="PVZ">CDEK PVZ (Self Pickup)</option>
            <option value="Courier">Courier Doorstep Delivery</option>
            <option value="Postal">Russian Post Relay</option>
          </select>
        </div>
      </div>
      <div class="flex justify-end gap-3 mt-4 pt-3 border-t border-[#38332b]">
        <button class="vault-btn-outline px-4 py-1.5 text-xs uppercase" onclick={() => { newAddressModalOpen = false; }}>Cancel</button>
        <button class="vault-btn-gold px-4 py-1.5 text-xs uppercase font-bold text-black" onclick={handleAddAddress}>Save Address</button>
      </div>
    </CornerBrackets>
  {/if}
</main>
