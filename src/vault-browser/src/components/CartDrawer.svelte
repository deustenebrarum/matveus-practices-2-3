<script lang="ts">
  import { cart } from '../lib/state/cart.svelte';
  import { ui } from '../lib/state/ui.svelte';
  import PuritySealBanner from './ui/PuritySealBanner.svelte';
  import VaultButton from './ui/VaultButton.svelte';

  let promoInput = $state('');

  function handleApplyPromo() {
    const res = cart.applyPromo(promoInput);
    if (res.success) {
      ui.notify(res.message, 'emerald');
    } else {
      ui.notify(res.message, 'crimson');
    }
  }

  function handleCheckout() {
    if (cart.items.length === 0) {
      ui.notify('Your requisition order is currently empty, Commander.', 'crimson');
      return;
    }
    ui.closeCart();
    ui.navigateTo('checkout');
  }
</script>

<svelte:window onkeydown={(e) => { if (e.key === 'Escape' && ui.cartDrawerOpen) ui.closeCart(); }} />

<!-- Drawer Backdrop -->
{#if ui.cartDrawerOpen}
  <div
    class="fixed inset-0 bg-black/60 backdrop-blur-sm z-40 transition-opacity"
    id="cartBackdrop"
    role="presentation"
    onclick={() => ui.closeCart()}
  ></div>
{/if}

<!-- Interactive Slide-out Requisition Drawer (matching design/index.html) -->
<aside
  aria-label="Requisition Order Drawer"
  class="fixed inset-y-0 right-0 z-50 w-full sm:w-[460px] bg-[#12141c] border-l-2 border-[#5a482b] shadow-2xl transform transition-transform duration-300 flex flex-col {ui.cartDrawerOpen ? 'translate-x-0' : 'translate-x-full'}"
  id="cartDrawer"
>
  <!-- Drawer Header with Aquila & Seal -->
  <div class="p-4 bg-[#161924] border-b border-[#3b3221] flex items-center justify-between">
    <div class="flex items-center gap-3">
      <svg class="w-8 h-7 text-vault-brightGold drop-shadow" fill="currentColor" viewBox="0 0 64 64">
        <path d="M32 6L36 16L48 12L42 22L58 20L48 30L62 34L46 40L56 52L40 46L36 60L32 48L28 60L24 46L8 52L18 40L2 34L16 30L6 20L22 22L16 12L28 16L32 6Z"></path>
      </svg>
      <div>
        <h2 class="font-cinzel text-base font-black tracking-widest text-vault-brightGold">REQUISITION ORDER</h2>
        <span class="text-[9px] font-mono uppercase text-gray-400">Sector Imperialis Munitorum</span>
      </div>
    </div>
    <!-- Close Drawer Button -->
    <button
      aria-label="Close cart"
      class="w-8 h-8 rounded border border-[#52442c] text-vault-gold hover:text-white hover:border-vault-brightGold flex items-center justify-center transition-colors text-sm cursor-pointer"
      id="closeCartBtn"
      onclick={() => ui.closeCart()}
    >
      ✕
    </button>
  </div>

  <!-- Requisition Item List -->
  <div class="flex-1 overflow-y-auto p-4 space-y-4" id="cartItemsContainer">
    {#if cart.items.length === 0}
      <div class="h-64 flex flex-col items-center justify-center text-center p-6 border border-[#2e2617] bg-[#0c0e14]">
        <div class="text-3xl text-vault-gold mb-2">✠</div>
        <h3 class="font-cinzel text-sm font-bold text-gray-300 uppercase tracking-wider mb-1">
          Arsenal Manifest Empty
        </h3>
        <p class="text-xs text-gray-500 font-sans mb-4">
          No wargear or miniatures have been designated for requisition yet.
        </p>
        <VaultButton
          variant="outline"
          size="sm"
          onclick={() => { ui.closeCart(); ui.navigateTo('catalog'); }}
        >
          Explore Catalog
        </VaultButton>
      </div>
    {:else}
      {#each cart.items as item (item.id + (item.wargear || ''))}
        <div class="flex gap-3 bg-[#171a24] p-3 rounded border border-[#3b3220] relative group">
          <div class="w-16 h-16 bg-black rounded overflow-hidden shrink-0 border border-[#2f2719]">
            <img
              alt={item.name}
              class="w-full h-full object-cover object-top"
              src={item.imageUrl || 'https://lh3.googleusercontent.com/aida-public/AB6AXuAvrktdY3q6xdAIuPjE0b34GVMYg9lwLvNdBQNC7loqq9AxB3EaWDhCfqORxN01S6iAY-hceugVnndPPZOUvnP0CWBtJmgk3XfRWZsQpjX9RzGUC37ltKBzZVcLI6IHE8CobjltV46D0D-o4N1HcXbU3IpPndAKvMkVE3KKqlId-z6mc5dMiLBKICoAmMJzKnJg3Q2VFGDKmiaU9CtXY9OlC2-7BVAhmLLK2QJDHrVlqJp_kcDwquU'}
            />
          </div>
          <div class="flex-1 flex flex-col justify-between">
            <div class="flex items-start justify-between">
              <h4 class="font-cinzel text-xs font-bold text-gray-100">
                {item.name}
              </h4>
              <button
                class="text-gray-500 hover:text-red-400 text-xs px-1 cursor-pointer"
                aria-label="Remove item"
                onclick={() => cart.removeItem(item.id, item.wargear)}
              >
                ✕
              </button>
            </div>
            <p class="text-[10px] text-gray-400">
              {item.wargear ? item.wargear : `${item.scale || '32mm'} • Citadel Multi-part`}
            </p>
            <div class="flex items-center justify-between mt-2">
              <div class="flex items-center bg-[#0e1017] border border-[#3d331f] rounded text-[11px]">
                <button
                  class="px-2 py-0.5 text-gray-400 hover:text-white cursor-pointer"
                  aria-label="Decrease quantity"
                  onclick={() => cart.updateQuantity(item.id, -1, item.wargear)}
                >
                  −
                </button>
                <span class="px-2 text-vault-brightGold font-mono item-qty">
                  {item.quantity}
                </span>
                <button
                  class="px-2 py-0.5 text-gray-400 hover:text-white cursor-pointer"
                  aria-label="Increase quantity"
                  onclick={() => cart.updateQuantity(item.id, 1, item.wargear)}
                >
                  +
                </button>
              </div>
              <span class="font-mono text-xs font-bold text-vault-brightGold item-price">
                ${(item.price * item.quantity).toFixed(2)}
              </span>
            </div>
          </div>
        </div>
      {/each}

      <!-- Purity Seal Discount Banner -->
      {#if cart.hasBundleDiscount}
        <PuritySealBanner
          title="Chapter Requisition Rite"
          subtitle="Bundle Set Discount Applied: -15%"
          discountText="-15%"
        />
      {/if}
    {/if}
  </div>

  <!-- Drawer Footer: Promocode & Calculations -->
  <div class="p-4 bg-[#161924] border-t border-[#3b3221] space-y-3">
    <!-- Promo Code Entry -->
    <div class="flex gap-2">
      <input
        class="flex-1 bg-[#0e1017] border border-[#3b3221] text-xs px-3 py-2 rounded uppercase tracking-wider text-gray-200 placeholder-gray-500 focus:border-vault-gold focus:ring-0"
        id="promoInput"
        placeholder="Promo code (e.g., TERRA10)"
        type="text"
        bind:value={promoInput}
        onkeydown={(e) => { if (e.key === 'Enter') handleApplyPromo(); }}
      />
      <VaultButton
        variant="outline"
        size="sm"
        onclick={handleApplyPromo}
      >
        Apply
      </VaultButton>
    </div>

    {#if cart.promoMessage}
      <div class="text-[10px] font-mono {cart.promoDiscountRate > 0 ? 'text-emerald-400' : 'text-red-400'}">
        {cart.promoMessage}
      </div>
    {/if}

    <!-- Price Breakdown -->
    <div class="space-y-1.5 text-xs pt-1 border-t border-[#262c3b]">
      <div class="flex justify-between text-gray-400">
        <span>Subtotal:</span>
        <span class="font-mono text-gray-200" id="drawerSubtotal">
          ${cart.subtotal.toFixed(2)}
        </span>
      </div>

      {#if cart.bundleDiscount > 0}
        <div class="flex justify-between text-emerald-400">
          <span>Bundle Discount (-15%):</span>
          <span class="font-mono" id="drawerDiscount">
            -${cart.bundleDiscount.toFixed(2)}
          </span>
        </div>
      {/if}

      {#if cart.promoDiscount > 0}
        <div class="flex justify-between text-emerald-400">
          <span>Promo Voucher ({cart.promoCode}):</span>
          <span class="font-mono">
            -${cart.promoDiscount.toFixed(2)}
          </span>
        </div>
      {/if}

      <div class="flex justify-between text-sm font-cinzel font-bold text-vault-brightGold pt-1 border-t border-[#262c3b]">
        <span>Total Order:</span>
        <span class="font-mono text-base font-bold" id="drawerTotal">
          ${cart.total.toFixed(2)}
        </span>
      </div>
    </div>

    <!-- Proceed Checkout CTA -->
    <VaultButton
      variant="gold"
      size="lg"
      class="w-full shadow-lg hover:shadow-[0_0_20px_rgba(223,185,108,0.4)]"
      onclick={handleCheckout}
    >
      <span>PROCEED TO CHECKOUT</span>
      <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path d="M14 5l7 7m0 0l-7 7m7-7H3" stroke-linecap="round" stroke-linejoin="round" stroke-width="2"></path>
      </svg>
    </VaultButton>
    <p class="text-[9px] text-center text-gray-500 uppercase tracking-widest font-cinzel">
      Blessed by the Administratum • Priority Imperial Shipping
    </p>
  </div>
</aside>
