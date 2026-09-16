<script lang="ts">
  import { cart } from '../lib/state/cart.svelte';
  import { ui } from '../lib/state/ui.svelte';

  let qty = $state(1);
  let selectedWargearIndex = $state(0);
  let activeImageIndex = $state(0);

  const product = $derived(ui.activeModalProduct);

  const galleryImages = $derived(
    product
      ? [
          product.imageUrl,
          'https://lh3.googleusercontent.com/aida-public/AB6AXuAxdgdVZGFOayuDOcE1JyJlmo6gof5ziIHYy4g6XatRCePR-lHl-6hFYZdlCI0DVdnmlJcdBHtSb_AuqnzjbVuOqx2W-wqH5xlj2E7PdEosDrP1lb85aOW6ZVy3UYB8BfKEZxRPdPMXIUtZKzabZEZm7Xd3y-1-WH6jCWJOY5NVE_H_XOQvEj4TYzMWlSjQ7Jg1SSOse4KOhm1ZZ5br09SvwL_uLW265wTKG7r-kXLfpSNHKZ596gU',
          'https://lh3.googleusercontent.com/aida-public/AB6AXuAz1attuzdjflfgitzCn4fbXz-6QEgFGhJGSbD-PtyMB90o2k1VKHbMzNFZN_CjBxCQ-g_l3Da2Or98--taVFSmLi2EDxi-NvxD31eH6YrYJFlm-hhdHTJogoZcwO-hA7ebiVyrCDfRlNYLh857ncySkptNI09PTA3RfsZj2NT4aDvMrNP-Z0TOaqGK9uPZTIfRgrbzjpFDyD0mknmNZGV4eG2j0RSTU5aRmOwTTtdvKctnTQdtci8',
          'https://lh3.googleusercontent.com/aida-public/AB6AXuDd1VsfPI_ib9nS9vHk86SDlQjqmsDC_tL7v-iWUDkSokjjR_1VnmoYviAQLAGHnqzCZeZGtFEzLdne2XeyZ_vxELDjjjvrEEMgaini5NwjscdprZmHdAv2Di3S9YBW3Dz1Ibj0wn9PemMl5zLPPySIhnY3mbc-AioRZ6ftu18RamNPT4D73VenbmaicAQ1r2wSmEORxu69VWpqYC-4cRKG5DrjQ4l1GrxGN4GBcDdcrbBOKXpJTec'
        ]
      : []
  );

  const selectedWargearName = $derived.by(() => {
    if (!product || !product.wargearOptions || product.wargearOptions.length === 0) {
      return 'Standard Battlefield Loadout';
    }
    const opt = product.wargearOptions[selectedWargearIndex] || product.wargearOptions[0];
    return opt.name;
  });

  const totalPrice = $derived(
    product ? (product.price * qty).toFixed(2) : '0.00'
  );

  function incrementQty() {
    qty++;
  }

  function decrementQty() {
    if (qty > 1) qty--;
  }

  function handleAddToCart() {
    if (!product) return;
    cart.addItem({
      id: product.id,
      name: product.name,
      price: product.price,
      quantity: qty,
      imageUrl: product.imageUrl,
      faction: product.faction,
      scale: product.scale,
      wargear: selectedWargearName,
      isStarterSet: product.tags.includes('StarterSet')
    });

    ui.pulseCart();
    ui.notify(`Added ${qty}x ${product.name} to requisition roster.`, 'gold');
    ui.closeProductModal();
    ui.openCart();
    qty = 1;
  }
</script>

<svelte:window onkeydown={(e) => { if (e.key === 'Escape' && ui.activeModalProduct) ui.closeProductModal(); }} />

{#if product}
  <!-- Grimdark Product Details Modal (Recreated directly from design/index.html) -->
  <div
    aria-modal="true"
    class="fixed inset-0 z-50 flex items-center justify-center p-4 transition-opacity duration-300"
    role="dialog"
  >
    <!-- Backdrop button -->
    <button
      type="button"
      class="fixed inset-0 bg-black/85 backdrop-blur-sm w-full h-full cursor-default border-none"
      onclick={() => ui.closeProductModal()}
      aria-label="Close dialog"
    ></button>

    <!-- Modal Dialog Box -->
    <div
      class="vault-frame-ornate bg-[#12141c] w-full max-w-4xl rounded-sm p-6 relative max-h-[92vh] overflow-y-auto z-10"
    >
      <!-- Close Button -->
      <button
        aria-label="Close modal"
        class="absolute top-3 right-3 text-vault-gold hover:text-white transition-colors p-1 z-20"
        onclick={() => ui.closeProductModal()}
      >
        <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path d="M6 18L18 6M6 6l12 12" stroke-linecap="round" stroke-linejoin="round" stroke-width="2"></path>
        </svg>
      </button>

      <!-- Modal Title -->
      <div class="text-center pb-4 mb-4 border-b border-[#3e3422]">
        <h2 class="font-cinzel text-xl sm:text-2xl font-bold tracking-widest text-[#e2be74] drop-shadow uppercase" id="modalProductTitle">
          {product.name}
        </h2>
      </div>

      <!-- Modal Content Grid -->
      <div class="grid grid-cols-1 md:grid-cols-12 gap-6 items-start">
        <!-- Left: Image Gallery (5 Cols) -->
        <div class="md:col-span-5 flex flex-col gap-3">
          <div class="relative w-full h-72 bg-[#0a0c10] border border-[#4d3f26] rounded overflow-hidden flex items-center justify-center group shadow-inner">
            <img
              alt={product.name}
              class="w-full h-full object-cover object-center transition-all duration-300"
              id="modalMainImg"
              src={galleryImages[activeImageIndex]}
            />
            <span class="absolute bottom-2 left-2 bg-black/70 px-2 py-0.5 text-[9px] font-mono text-vault-brightGold border border-vault-gold/40">
              HIGH RESOLUTION MODEL
            </span>
          </div>

          <!-- Gallery Thumbnails -->
          <div class="grid grid-cols-4 gap-2">
            {#each galleryImages as img, idx}
              <button
                class="h-16 bg-[#0c0e14] border-2 {activeImageIndex === idx ? 'border-vault-gold' : 'border-[#3b3221] hover:border-vault-gold/70'} rounded overflow-hidden focus:outline-none transition-colors"
                onclick={() => { activeImageIndex = idx; }}
              >
                <img alt="Thumbnail {idx + 1}" class="w-full h-full object-cover object-top" src={img} />
              </button>
            {/each}
          </div>
        </div>

        <!-- Right: Technical Specs & Deployment Config (7 Cols) -->
        <div class="md:col-span-7 flex flex-col gap-4">
          <!-- Faction Crest & Badging -->
          <div class="flex items-center justify-between">
            <div class="flex items-center gap-3">
              <div class="w-10 h-10 rounded-full border border-vault-gold flex items-center justify-center bg-red-950/40 text-vault-brightGold font-cinzel font-bold text-lg select-none">
                ✠
              </div>
              <div>
                <h4 class="font-cinzel text-sm font-bold text-white uppercase tracking-wider" id="modalFaction">
                  {product.faction}
                </h4>
                <p class="text-[11px] text-gray-400">
                  {product.subfaction || product.name} | {product.scale} Miniature
                </p>
              </div>
            </div>
            <!-- Citadel Authenticity Seal -->
            <div class="text-right">
              <span class="font-cinzel font-black tracking-tighter text-xs text-gray-300 block">CITADEL</span>
              <span class="text-[9px] uppercase tracking-widest text-vault-gold font-mono block">MINIATURES</span>
            </div>
          </div>

          <!-- Specs Table -->
          <div class="grid grid-cols-3 gap-2 bg-[#171a24] p-2.5 rounded border border-[#363022] text-center">
            <div>
              <span class="text-[10px] font-cinzel uppercase text-gray-400 block">SCALE</span>
              <span class="text-xs font-mono font-bold text-vault-brightGold" id="modalScale">
                {product.scale}
              </span>
            </div>
            <div class="border-x border-[#363022]">
              <span class="text-[10px] font-cinzel uppercase text-gray-400 block">POINTS COST</span>
              <span class="text-xs font-mono font-bold text-emerald-400" id="modalPoints">
                {product.points || '85 pts'}
              </span>
            </div>
            <div>
              <span class="text-[10px] font-cinzel uppercase text-gray-400 block">FACTION</span>
              <span class="text-xs font-cinzel font-bold text-gray-200">
                {product.faction}
              </span>
            </div>
          </div>

          <!-- Wargear Customization -->
          <div>
            <label class="block text-xs font-cinzel font-bold text-vault-brightGold uppercase mb-1" for="modalWargearSelect">
              WARGEAR CUSTOMIZATION
            </label>
            <div class="relative">
              <select
                class="w-full bg-[#171a24] border border-[#4d3f26] text-xs text-gray-200 py-2 px-3 rounded appearance-none focus:border-vault-gold focus:ring-0 cursor-pointer"
                id="modalWargearSelect"
                bind:value={selectedWargearIndex}
              >
                {#if product.wargearOptions && product.wargearOptions.length > 0}
                  {#each product.wargearOptions as opt, idx}
                    <option value={idx}>
                      {opt.name} {opt.pointsCost > 0 ? `(+${opt.pointsCost} pts)` : ''}
                    </option>
                  {/each}
                {:else}
                  <option value={0}>Standard Tactical Armament</option>
                {/if}
              </select>
              <div class="pointer-events-none absolute inset-y-0 right-0 flex items-center px-3 text-vault-gold">
                ▼
              </div>
            </div>
          </div>

          <!-- Lore Snippet -->
          <div>
            <span class="text-[11px] font-cinzel font-bold uppercase tracking-wider text-vault-gold block mb-1">
              LORE SNIPPET
            </span>
            <p class="text-xs text-gray-400 leading-relaxed italic bg-[#0c0d12] p-2.5 rounded border border-[#232630]">
              "{product.description}"
            </p>
          </div>

          <!-- Stock Availability -->
          <div class="flex items-center justify-between text-xs pt-1">
            <span class="font-cinzel text-gray-400 uppercase">STOCK AVAILABILITY:</span>
            <span class="font-mono text-emerald-400 font-bold" id="modalStock">
              IN STOCK: {product.stock ?? 5} PCS
            </span>
          </div>

          <!-- Quantity Selector and Add-to-Cart Action -->
          <div class="flex items-center gap-3 pt-2">
            <!-- Stepper -->
            <div class="flex items-center bg-[#171a24] border border-[#4d3f26] rounded">
              <button
                class="px-3 py-2 text-gray-400 hover:text-white font-bold transition-colors"
                onclick={decrementQty}
              >
                −
              </button>
              <span class="px-3 py-2 text-xs font-mono font-bold text-vault-brightGold min-w-[28px] text-center" id="modalQtyDisplay">
                {qty}
              </span>
              <button
                class="px-3 py-2 text-gray-400 hover:text-white font-bold transition-colors"
                onclick={incrementQty}
              >
                +
              </button>
            </div>

            <!-- Primary Add Button -->
            <button
              class="vault-btn-gold flex-1 py-2.5 px-4 text-xs tracking-wider uppercase font-bold text-black flex items-center justify-center gap-2"
              id="modalAddToCartBtn"
              onclick={handleAddToCart}
            >
              <span>ADD TO CART</span>
              <span class="text-black/60">•</span>
              <span id="modalBtnPrice">${totalPrice}</span>
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
{/if}
