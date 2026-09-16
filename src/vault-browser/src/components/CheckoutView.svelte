<script lang="ts">
  import { cart } from '../lib/state/cart.svelte';
  import { ui } from '../lib/state/ui.svelte';
  import { user } from '../lib/state/user.svelte';
  import { createOrder } from '../lib/api';
  import type { Order } from '../types';
  import VaultCard from './ui/VaultCard.svelte';
  import VaultButton from './ui/VaultButton.svelte';
  import PuritySealBanner from './ui/PuritySealBanner.svelte';

  let fullName = $state(user.name);
  let email = $state(user.email);
  let phone = $state(user.phone);
  let city = $state('Moscow');
  let address = $state('CDEK PVZ #104, Tverskaya St, 12, bld. 2');
  let courierService = $state('CDEK Express');
  let submitting = $state(false);
  let createdOrder = $state<Order | null>(null);

  async function handleSubmitOrder(e: SubmitEvent) {
    e.preventDefault();
    if (cart.items.length === 0) {
      ui.notify('Your requisition manifest is empty.', 'crimson');
      return;
    }

    submitting = true;
    try {
      const order = await createOrder({
        customer: {
          fullName,
          email,
          phone,
          shippingAddress: address,
          city,
          courierService
        },
        items: cart.items.map(i => ({
          miniatureId: i.id,
          quantity: i.quantity
        })),
        promoCode: cart.promoCode || undefined
      });

      createdOrder = order;

      // Update user state active dispatch with newly sanctified order
      user.activeOrderNumber = order.orderNumber;
      user.activeOrderDate = new Date().toLocaleDateString('en-US', { month: 'short', day: 'numeric', year: 'numeric' });
      user.activeCourier = order.customer.courierService;
      user.activeDestination = `${order.customer.city}, ${order.customer.shippingAddress}`;
      user.activeStep = 1;

      cart.clearCart();
      ui.notify(`Order #${order.orderNumber} successfully registered and sanctified!`, 'emerald');
    } catch {
      ui.notify('Transmission failure. Please re-try requisition.', 'crimson');
    } finally {
      submitting = false;
    }
  }
</script>

<div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8 w-full">
  <!-- Breadcrumb -->
  <div class="flex items-center space-x-2 text-[11px] font-cinzel tracking-widest text-[#787265] uppercase mb-6">
    <button type="button" class="hover:text-vault-gold transition-colors cursor-pointer" onclick={() => ui.navigateTo('catalog')}>Catalog</button>
    <span>/</span>
    <span class="text-vault-gold">Requisition Protocol Checkout</span>
  </div>

  {#if createdOrder}
    <!-- Confirmation Banner Screen -->
    <VaultCard brackets={true} class="p-8 text-center max-w-2xl mx-auto border-2 border-vault-gold">
      <div class="w-16 h-16 mx-auto rounded-full bg-[#8c1b1b] shadow flex items-center justify-center text-[#ffc6c6] font-serif font-black text-2xl border-2 border-[#b53a3a] mb-4">
        ✠
      </div>
      <h2 class="font-cinzel text-2xl font-bold text-vault-brightGold uppercase tracking-widest mb-2">
        Requisition Sanctified
      </h2>
      <p class="text-xs text-gray-400 font-sans mb-4">
        The Imperial Munitorum has received your requisition mandate. Miniature sets have been reserved for dispatch.
      </p>

      <div class="bg-[#0e1017] p-4 border border-[#38332b] mb-6 text-left space-y-2 text-xs font-mono">
        <div class="flex justify-between">
          <span class="text-gray-400">SANCTIFIED ORDER NUMBER:</span>
          <span class="text-vault-brightGold font-bold">{createdOrder.orderNumber}</span>
        </div>
        <div class="flex justify-between">
          <span class="text-gray-400">COMMANDER:</span>
          <span class="text-gray-200">{createdOrder.customer.fullName}</span>
        </div>
        <div class="flex justify-between">
          <span class="text-gray-400">COURIER PROTOCOL:</span>
          <span class="text-emerald-400">{createdOrder.customer.courierService}</span>
        </div>
        <div class="flex justify-between border-t border-[#2a2f3d] pt-2 font-bold text-sm">
          <span class="text-vault-gold">TOTAL VALUE:</span>
          <span class="text-white">${createdOrder.totalAmount.toFixed(2)}</span>
        </div>
      </div>

      <div class="flex flex-col sm:flex-row justify-center gap-4">
        <VaultButton
          variant="gold"
          size="md"
          onclick={() => ui.navigateTo('account')}
        >
          View in Commander Dossier
        </VaultButton>
        <VaultButton
          variant="outline"
          size="md"
          onclick={() => ui.navigateTo('catalog')}
        >
          Return to Armory
        </VaultButton>
      </div>
    </VaultCard>
  {:else}
    <!-- Checkout Form & Summary Grid -->
    <div class="grid grid-cols-1 lg:grid-cols-12 gap-8 items-start">
      <!-- Left: Form (7 cols) -->
      <form class="lg:col-span-7 space-y-6" onsubmit={handleSubmitOrder}>
        <!-- Credentials Card -->
        <VaultCard brackets={true} class="p-6">
          <div class="pb-3 mb-4 border-b border-[#38332b]">
            <h3 class="font-cinzel text-sm font-bold text-vault-brightGold uppercase tracking-widest">
              1. Commander Credentials
            </h3>
          </div>

          <div class="space-y-4 text-xs">
            <div>
              <label class="block font-cinzel text-gray-300 uppercase mb-1" for="fullNameInput">
                Full Name / Callsign *
              </label>
              <input
                id="fullNameInput"
                required
                class="w-full bg-[#0e1017] border border-[#4d3f26] focus:border-vault-gold text-xs text-white p-2.5 focus:outline-none"
                bind:value={fullName}
              />
            </div>

            <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
              <div>
                <label class="block font-cinzel text-gray-300 uppercase mb-1" for="emailInput">
                  Comm-mail (Email) *
                </label>
                <input
                  id="emailInput"
                  type="email"
                  required
                  class="w-full bg-[#0e1017] border border-[#4d3f26] focus:border-vault-gold text-xs text-white p-2.5 focus:outline-none font-mono"
                  bind:value={email}
                />
              </div>
              <div>
                <label class="block font-cinzel text-gray-300 uppercase mb-1" for="phoneInput">
                  Comm-line (Phone) *
                </label>
                <input
                  id="phoneInput"
                  required
                  class="w-full bg-[#0e1017] border border-[#4d3f26] focus:border-vault-gold text-xs text-white p-2.5 focus:outline-none font-mono"
                  bind:value={phone}
                />
              </div>
            </div>
          </div>
        </VaultCard>

        <!-- Destination Card -->
        <VaultCard brackets={true} class="p-6">
          <div class="pb-3 mb-4 border-b border-[#38332b]">
            <h3 class="font-cinzel text-sm font-bold text-vault-brightGold uppercase tracking-widest">
              2. Deployment Destination & Logistics
            </h3>
          </div>

          <div class="space-y-4 text-xs">
            <div>
              <label class="block font-cinzel text-gray-300 uppercase mb-1" for="cityInput">
                Hive Sector / City *
              </label>
              <input
                id="cityInput"
                required
                class="w-full bg-[#0e1017] border border-[#4d3f26] focus:border-vault-gold text-xs text-white p-2.5 focus:outline-none"
                bind:value={city}
              />
            </div>

            <div>
              <label class="block font-cinzel text-gray-300 uppercase mb-1" for="addressInput">
                Pick-up Point (PVZ) or Street Address *
              </label>
              <input
                id="addressInput"
                required
                class="w-full bg-[#0e1017] border border-[#4d3f26] focus:border-vault-gold text-xs text-white p-2.5 focus:outline-none font-sans"
                bind:value={address}
              />
            </div>

            <div>
              <label class="block font-cinzel text-gray-300 uppercase mb-1" for="courierSelect">
                Sanctioned Courier Service
              </label>
              <select
                id="courierSelect"
                class="w-full bg-[#0e1017] border border-[#4d3f26] focus:border-vault-gold text-xs text-white p-2.5 focus:outline-none font-cinzel cursor-pointer"
                bind:value={courierService}
              >
                <option value="CDEK Express">CDEK Express (Rapid Sector Transport • 48h)</option>
                <option value="Direct Courier">Direct Courier Delivery (To Fortress Doorstep)</option>
                <option value="Russian Post">Russian Post Relay (Standard Imperial Freight)</option>
              </select>
            </div>
          </div>
        </VaultCard>

        <!-- Submit Button -->
        <VaultButton
          type="submit"
          variant="gold"
          size="lg"
          disabled={submitting}
          class="w-full shadow-xl hover:shadow-[0_0_20px_rgba(223,185,108,0.4)]"
        >
          <span>{submitting ? 'TRANSMITTING ORDER...' : 'CONFIRM SANCTIFIED REQUISITION'}</span>
          <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path d="M14 5l7 7m0 0l-7 7m7-7H3" stroke-linecap="round" stroke-linejoin="round" stroke-width="2"></path>
          </svg>
        </VaultButton>
      </form>

      <!-- Right: Order Summary (5 cols) -->
      <aside class="lg:col-span-5 space-y-6" aria-label="Requisition Manifest Summary">
        <VaultCard brackets={true} class="p-6">
          <div class="pb-3 mb-4 border-b border-[#38332b] flex items-center justify-between">
            <h3 class="font-cinzel text-sm font-bold text-vault-brightGold uppercase tracking-widest">
              Requisition Manifest
            </h3>
            <span class="text-xs font-mono text-gray-400">
              {cart.count} Units
            </span>
          </div>

          <div class="space-y-3 max-h-80 overflow-y-auto pr-1">
            {#each cart.items as item (item.id + (item.wargear || ''))}
              <div class="flex gap-3 bg-[#11131a] p-2.5 border border-[#2b2518]">
                <div class="w-12 h-12 bg-black rounded overflow-hidden shrink-0 border border-[#3b3221]">
                  <img alt={item.name} class="w-full h-full object-cover" src={item.imageUrl} />
                </div>
                <div class="flex-1 text-xs flex flex-col justify-between">
                  <div class="font-cinzel font-bold text-gray-200 truncate">
                    {item.name}
                  </div>
                  <div class="text-[10px] text-gray-400 font-mono">
                    Qty: {item.quantity} • ${(item.price * item.quantity).toFixed(2)}
                  </div>
                </div>
              </div>
            {/each}
          </div>

          {#if cart.hasBundleDiscount}
            <PuritySealBanner class="mt-4" />
          {/if}

          <!-- Cost Totals -->
          <div class="pt-4 mt-4 border-t border-[#38332b] space-y-2 text-xs">
            <div class="flex justify-between text-gray-400">
              <span>Subtotal:</span>
              <span class="font-mono text-gray-200">${cart.subtotal.toFixed(2)}</span>
            </div>
            {#if cart.bundleDiscount > 0}
              <div class="flex justify-between text-emerald-400">
                <span>Bundle Discount (-15%):</span>
                <span class="font-mono">-${cart.bundleDiscount.toFixed(2)}</span>
              </div>
            {/if}
            {#if cart.promoDiscount > 0}
              <div class="flex justify-between text-emerald-400">
                <span>Promo Code ({cart.promoCode}):</span>
                <span class="font-mono">-${cart.promoDiscount.toFixed(2)}</span>
              </div>
            {/if}
            <div class="flex justify-between text-base font-cinzel font-bold text-vault-brightGold pt-2 border-t border-[#2a2f3d]">
              <span>Final Tithe:</span>
              <span class="font-mono">${cart.total.toFixed(2)}</span>
            </div>
          </div>
        </VaultCard>
      </aside>
    </div>
  {/if}
</div>
