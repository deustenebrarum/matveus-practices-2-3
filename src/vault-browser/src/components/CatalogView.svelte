<script lang="ts">
  import { onMount } from 'svelte';
  import type { Miniature, FactionCount } from '../types';
  import { fetchMiniatures, fetchFactions, SEED_MINIATURES } from '../lib/api';
  import { cart } from '../lib/state/cart.svelte';
  import { ui } from '../lib/state/ui.svelte';
  import VaultBadge from './ui/VaultBadge.svelte';

  let miniatures = $state<Miniature[]>(SEED_MINIATURES);
  let factionCounts = $state<FactionCount[]>([]);
  let loading = $state<boolean>(true);

  // Filter state
  let selectedFactions = $state<string[]>([]);
  let selectedMaterials = $state<string[]>([]);
  let selectedScales = $state<string[]>([]);
  let maxPrice = $state<number>(250);
  let inStockOnly = $state<boolean>(false);
  let sortBy = $state<'featured' | 'price_asc' | 'price_desc' | 'name'>('featured');

  onMount(async () => {
    try {
      const [items, factions] = await Promise.all([
        fetchMiniatures(),
        fetchFactions()
      ]);
      miniatures = items;
      factionCounts = factions;
    } catch {
      // Fallback already assigned
    } finally {
      loading = false;
    }
  });

  function resetFilters() {
    selectedFactions = [];
    selectedMaterials = [];
    selectedScales = [];
    maxPrice = 250;
    inStockOnly = false;
    ui.searchQuery = '';
    ui.selectedUniverse = '';
  }

  function toggleFaction(f: string) {
    if (selectedFactions.includes(f)) {
      selectedFactions = selectedFactions.filter(x => x !== f);
    } else {
      selectedFactions = [...selectedFactions, f];
    }
  }

  function toggleMaterial(m: string) {
    if (selectedMaterials.includes(m)) {
      selectedMaterials = selectedMaterials.filter(x => x !== m);
    } else {
      selectedMaterials = [...selectedMaterials, m];
    }
  }

  function toggleScale(s: string) {
    if (selectedScales.includes(s)) {
      selectedScales = selectedScales.filter(x => x !== s);
    } else {
      selectedScales = [...selectedScales, s];
    }
  }

  const filteredMiniatures = $derived.by(() => {
    let list = miniatures;

    // Global Search Query
    if (ui.searchQuery.trim()) {
      const q = ui.searchQuery.trim().toLowerCase();
      list = list.filter(
        m =>
          m.name.toLowerCase().includes(q) ||
          m.faction.toLowerCase().includes(q) ||
          (m.subfaction && m.subfaction.toLowerCase().includes(q)) ||
          m.description.toLowerCase().includes(q) ||
          m.tags.some(t => t.toLowerCase().includes(q))
      );
    }

    // Universe filter
    if (ui.selectedUniverse) {
      list = list.filter(m => m.universe.toLowerCase().includes(ui.selectedUniverse.toLowerCase()));
    }

    // Faction filter
    if (selectedFactions.length > 0) {
      list = list.filter(m =>
        selectedFactions.some(f => m.faction.toLowerCase().includes(f.toLowerCase()))
      );
    }

    // Material filter
    if (selectedMaterials.length > 0) {
      list = list.filter(m =>
        selectedMaterials.some(mat => m.material.toLowerCase().includes(mat.toLowerCase()))
      );
    }

    // Scale filter
    if (selectedScales.length > 0) {
      list = list.filter(m =>
        selectedScales.some(sc => m.scale.toLowerCase().includes(sc.toLowerCase()))
      );
    }

    // Price filter
    list = list.filter(m => m.price <= maxPrice);

    // Stock filter
    if (inStockOnly) {
      list = list.filter(m => (m.stock ?? 1) > 0);
    }

    // Sorting
    return [...list].sort((a, b) => {
      if (sortBy === 'price_asc') return a.price - b.price;
      if (sortBy === 'price_desc') return b.price - a.price;
      if (sortBy === 'name') return a.name.localeCompare(b.name);
      // Default: featured first, then name
      if (a.isFeatured !== b.isFeatured) return a.isFeatured ? -1 : 1;
      return a.name.localeCompare(b.name);
    });
  });

  function quickAddToCart(e: MouseEvent, m: Miniature) {
    e.stopPropagation();
    cart.addItem({
      id: m.id,
      name: m.name,
      price: m.price,
      quantity: 1,
      imageUrl: m.imageUrl,
      faction: m.faction,
      scale: m.scale,
      isStarterSet: m.tags.includes('StarterSet')
    });
    ui.pulseCart();
    ui.notify(`Added ${m.name} to Cart.`, 'gold');
  }
</script>

<!-- Universe Quick Switcher Bar -->
<div class="border-b border-[#38332b] bg-[#0c0d11]/80 py-3 mb-6">
  <div class="max-w-[1440px] mx-auto px-4 sm:px-6 flex flex-wrap items-center justify-between gap-3">
    <div class="flex items-center gap-2 text-xs font-cinzel tracking-wider">
      <span class="text-gray-400 uppercase text-[11px] mr-1">Sector:</span>
      <button
        class="px-3 py-1 rounded-none border transition-colors {ui.selectedUniverse === '' ? 'bg-[#231b12] border-vault-gold text-vault-brightGold font-bold' : 'bg-[#12141c] border-[#38332b] text-gray-400 hover:text-white'}"
        onclick={() => ui.setUniverse('')}
      >
        All Sectors
      </button>
      <button
        class="px-3 py-1 rounded-none border transition-colors {ui.selectedUniverse === 'Warhammer 40,000' ? 'bg-[#231b12] border-vault-gold text-vault-brightGold font-bold' : 'bg-[#12141c] border-[#38332b] text-gray-400 hover:text-white'}"
        onclick={() => ui.setUniverse('Warhammer 40,000')}
      >
        Warhammer 40,000
      </button>
      <button
        class="px-3 py-1 rounded-none border transition-colors {ui.selectedUniverse === 'Age of Sigmar' ? 'bg-[#231b12] border-vault-gold text-vault-brightGold font-bold' : 'bg-[#12141c] border-[#38332b] text-gray-400 hover:text-white'}"
        onclick={() => ui.setUniverse('Age of Sigmar')}
      >
        Age of Sigmar
      </button>
    </div>

    {#if ui.searchQuery}
      <div class="flex items-center gap-2 text-xs font-mono text-vault-brightGold bg-[#14151a] px-3 py-1 border border-[#38332b]">
        <span>Filtering: "{ui.searchQuery}"</span>
        <button class="text-gray-400 hover:text-white ml-1 font-bold" onclick={() => ui.setSearch('')}>✕</button>
      </div>
    {/if}
  </div>
</div>

<!-- BEGIN: MainStoreLayout matching design/index.html -->
<main class="max-w-[1440px] w-full mx-auto px-4 sm:px-6 py-2 flex-1 flex flex-col md:flex-row gap-6">
  <!-- BEGIN: LeftFilterRail -->
  <aside class="w-full md:w-64 shrink-0 space-y-6">
    <div class="bg-[#12141c] border border-[#3b3220] p-4 rounded shadow-xl">
      <!-- Filter Header -->
      <div class="flex items-center justify-between pb-3 border-b border-[#2d281c] mb-4">
        <h2 class="font-cinzel text-sm font-bold tracking-widest text-vault-brightGold flex items-center gap-2">
          <span>FILTERS</span>
        </h2>
        <button
          class="text-[10px] uppercase font-cinzel text-gray-400 hover:text-vault-gold transition-colors"
          onclick={resetFilters}
        >
          Reset All
        </button>
      </div>

      <!-- Filter Group: Factions -->
      <div class="mb-5 border-b border-[#232632] pb-4">
        <div class="flex items-center justify-between text-xs font-cinzel font-bold text-gray-200 uppercase mb-2.5">
          <span class="flex items-center gap-1.5 text-vault-brightGold">FACTIONS</span>
          <span class="text-gray-500 text-sm">▴</span>
        </div>
        <ul class="space-y-2 text-xs">
          {#each [
            { name: 'Imperium', count: 14 },
            { name: 'Chaos', count: 9 },
            { name: 'Xenos', count: 12 },
            { name: 'Age of Sigmar', count: 6 }
          ] as f}
            {@const checked = selectedFactions.includes(f.name)}
            <li class="flex items-center justify-between text-gray-300 hover:text-white">
              <button
                type="button"
                role="checkbox"
                aria-checked={checked}
                class="flex items-center justify-between w-full text-left cursor-pointer group text-xs bg-transparent border-none p-0"
                onclick={() => toggleFaction(f.name)}
              >
                <span class="flex items-center gap-2.5">
                  <span class="w-4 h-4 rounded-sm {checked ? 'bg-[#12141c] border-vault-gold text-vault-brightGold shadow-[0_0_6px_rgba(223,185,108,0.4)]' : 'bg-[#0c0d12] border-[#3e3422] group-hover:border-vault-gold/70'} border flex items-center justify-center transition-colors">
                    {#if checked}
                      <svg class="w-3 h-3 text-vault-brightGold" viewBox="0 0 24 24" fill="currentColor">
                        <path d="M10 2h4v7h7v4h-7v7h-4v-7H3v-4h7V2z"></path>
                      </svg>
                    {/if}
                  </span>
                  <span class="group-hover:translate-x-0.5 transition-transform {checked ? 'text-white font-medium' : 'text-gray-400 group-hover:text-gray-200'}">
                    {f.name}
                  </span>
                </span>
                <span class="text-[10px] {checked ? 'text-vault-gold' : 'text-gray-500'} font-mono">
                  {f.count}
                </span>
              </button>
            </li>
          {/each}
        </ul>
      </div>

      <!-- Filter Group: Material -->
      <div class="mb-5 border-b border-[#232632] pb-4">
        <div class="flex items-center justify-between text-xs font-cinzel font-bold text-gray-200 uppercase mb-2.5">
          <span class="flex items-center gap-1.5 text-vault-brightGold">MATERIAL</span>
          <span class="text-gray-500 text-sm">▴</span>
        </div>
        <ul class="space-y-2 text-xs">
          {#each ['Citadel Plastic', 'Finecast Resin', 'White Metal'] as mat}
            {@const checked = selectedMaterials.includes(mat)}
            <li class="flex items-center justify-between text-gray-300 hover:text-white">
              <button
                type="button"
                role="checkbox"
                aria-checked={checked}
                class="flex items-center gap-2.5 cursor-pointer w-full text-left text-xs bg-transparent border-none p-0 group"
                onclick={() => toggleMaterial(mat)}
              >
                <span class="w-4 h-4 rounded-sm {checked ? 'bg-[#12141c] border-vault-gold text-vault-brightGold shadow-[0_0_6px_rgba(223,185,108,0.4)]' : 'bg-[#0c0d12] border-[#3e3422] group-hover:border-vault-gold/70'} border flex items-center justify-center transition-colors">
                  {#if checked}
                    <svg class="w-3 h-3 text-vault-brightGold" viewBox="0 0 24 24" fill="currentColor">
                      <path d="M10 2h4v7h7v4h-7v7h-4v-7H3v-4h7V2z"></path>
                    </svg>
                  {/if}
                </span>
                <span class="group-hover:translate-x-0.5 transition-transform {checked ? 'text-white font-medium' : 'text-gray-400 group-hover:text-gray-200'}">
                  {mat}
                </span>
              </button>
            </li>
          {/each}
        </ul>
      </div>

      <!-- Filter Group: Scale -->
      <div class="mb-5 border-b border-[#232632] pb-4">
        <div class="flex items-center justify-between text-xs font-cinzel font-bold text-gray-200 uppercase mb-2.5">
          <span class="flex items-center gap-1.5 text-vault-brightGold">SCALE</span>
          <span class="text-gray-500 text-sm">▴</span>
        </div>
        <ul class="space-y-2 text-xs">
          {#each ['28mm Heroic', '32mm Standard', '40mm Terminator', 'Vehicle & Monster'] as sc}
            {@const checked = selectedScales.includes(sc)}
            <li class="flex items-center justify-between text-gray-300 hover:text-white">
              <button
                type="button"
                role="checkbox"
                aria-checked={checked}
                class="flex items-center gap-2.5 cursor-pointer w-full text-left text-xs bg-transparent border-none p-0 group"
                onclick={() => toggleScale(sc)}
              >
                <span class="w-4 h-4 rounded-sm {checked ? 'bg-[#12141c] border-vault-gold text-vault-brightGold shadow-[0_0_6px_rgba(223,185,108,0.4)]' : 'bg-[#0c0d12] border-[#3e3422] group-hover:border-vault-gold/70'} border flex items-center justify-center transition-colors">
                  {#if checked}
                    <svg class="w-3 h-3 text-vault-brightGold" viewBox="0 0 24 24" fill="currentColor">
                      <path d="M10 2h4v7h7v4h-7v7h-4v-7H3v-4h7V2z"></path>
                    </svg>
                  {/if}
                </span>
                <span class="group-hover:translate-x-0.5 transition-transform {checked ? 'text-white font-medium' : 'text-gray-400 group-hover:text-gray-200'}">
                  {sc}
                </span>
              </button>
            </li>
          {/each}
        </ul>
      </div>

      <!-- Filter Group: Price Range -->
      <div class="mb-5 border-b border-[#232632] pb-4">
        <div class="flex items-center justify-between text-xs font-cinzel font-bold text-gray-200 uppercase mb-2.5">
          <span class="text-vault-brightGold">PRICE RANGE</span>
          <span class="text-xs text-vault-gold font-mono" id="sliderVal">$10 - ${maxPrice}</span>
        </div>
        <input
          class="vault-slider w-full cursor-pointer"
          max="250"
          min="10"
          type="range"
          bind:value={maxPrice}
        />
        <div class="flex justify-between text-[10px] font-mono text-gray-500 mt-1">
          <span>$10</span>
          <span>$250</span>
        </div>
      </div>

      <!-- Filter Group: Availability -->
      <div class="mb-2">
        <div class="flex items-center justify-between text-xs font-cinzel font-bold text-gray-200 uppercase mb-2.5">
          <span class="text-vault-brightGold">AVAILABILITY</span>
          <span class="text-gray-500 text-sm">▴</span>
        </div>
        <ul class="space-y-2 text-xs">
          <li class="flex items-center justify-between text-gray-300 hover:text-white">
            <button
              type="button"
              role="checkbox"
              aria-checked={inStockOnly}
              class="flex items-center gap-2.5 cursor-pointer w-full text-left text-xs bg-transparent border-none p-0 group"
              onclick={() => { inStockOnly = !inStockOnly; }}
            >
              <span class="w-4 h-4 rounded-sm {inStockOnly ? 'bg-[#12141c] border-vault-gold text-vault-brightGold shadow-[0_0_6px_rgba(223,185,108,0.4)]' : 'bg-[#0c0d12] border-[#3e3422] group-hover:border-vault-gold/70'} border flex items-center justify-center transition-colors">
                {#if inStockOnly}
                  <svg class="w-3 h-3 text-vault-brightGold" viewBox="0 0 24 24" fill="currentColor">
                    <path d="M10 2h4v7h7v4h-7v7h-4v-7H3v-4h7V2z"></path>
                  </svg>
                {/if}
              </span>
              <span class="group-hover:translate-x-0.5 transition-transform {inStockOnly ? 'text-white font-medium' : 'text-gray-400 group-hover:text-gray-200'}">
                In Stock Only
              </span>
            </button>
          </li>
        </ul>
      </div>
    </div>
  </aside>
  <!-- END: LeftFilterRail -->

  <!-- BEGIN: ProductCatalogArea -->
  <section class="flex-1">
    <!-- Catalog Controls & Metrics -->
    <div class="flex items-center justify-between pb-3 mb-4 border-b border-[#2d281c]">
      <span class="text-xs font-cinzel text-gray-400">
        Discovered <strong class="text-vault-brightGold font-mono">{filteredMiniatures.length}</strong> Relics
      </span>
      <div class="flex items-center gap-3">
        <span class="text-xs text-gray-400 font-cinzel uppercase tracking-widest font-semibold">Sort by:</span>
        <div class="relative group flex items-center bg-[#12141c] border border-[#5a482b] hover:border-vault-gold/70 focus-within:border-vault-gold rounded-none transition-colors shadow-inner">
          <select
            class="appearance-none bg-transparent text-xs font-cinzel tracking-wider text-[#e2be74] py-1.5 pl-3 pr-8 focus:outline-none cursor-pointer uppercase font-medium"
            bind:value={sortBy}
          >
            <option value="featured" class="bg-[#0f1117] text-gray-200 py-1">Standard Dispatch (Featured)</option>
            <option value="price_asc" class="bg-[#0f1117] text-gray-200 py-1">Points / Price: Low to High</option>
            <option value="price_desc" class="bg-[#0f1117] text-gray-200 py-1">Points / Price: High to Low</option>
            <option value="name" class="bg-[#0f1117] text-gray-200 py-1">Unit Codename (A-Z)</option>
          </select>
          <div class="pointer-events-none absolute inset-y-0 right-0 flex items-center pr-2.5 text-vault-brightGold group-hover:text-amber-300 transition-colors">
            <svg class="w-3.5 h-3.5" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round">
              <polyline points="6 9 12 15 18 9"></polyline>
            </svg>
          </div>
        </div>
      </div>
    </div>

    <!-- Miniature Products Grid matching visual reference -->
    {#if filteredMiniatures.length === 0}
      <div class="p-12 text-center border border-[#3b3221] bg-[#12141c] my-6">
        <div class="text-4xl text-vault-gold mb-3">✠</div>
        <h3 class="font-cinzel text-base font-bold text-gray-200 uppercase tracking-widest mb-1">
          No Sanctioned Miniatures Found
        </h3>
        <p class="text-xs text-gray-400 font-sans mb-4">
          No records match the requested sector, faction or price range filters.
        </p>
        <button class="vault-btn-gold px-4 py-2 text-xs uppercase" onclick={resetFilters}>
          Clear Filters
        </button>
      </div>
    {:else}
      <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4 md:gap-5" id="miniaturesCatalog">
        {#each filteredMiniatures as m (m.id)}
          <div
            class="vault-card rounded p-3 flex flex-col justify-between group cursor-pointer"
            role="button"
            tabindex="0"
            onclick={() => ui.openProductModal(m)}
            onkeydown={(e) => { if (e.key === 'Enter') ui.openProductModal(m); }}
          >
            <!-- Image Area with badges -->
            <div class="w-full h-48 bg-[#090b0e] rounded overflow-hidden relative border border-[#232630] flex items-center justify-center group-hover:border-[#5a482b] transition-colors">
              <img
                alt={m.name}
                class="w-full h-full object-cover object-top filter brightness-95 contrast-105 group-hover:scale-105 transition-transform duration-300"
                src={m.imageUrl}
              />
              <span class="absolute top-2 left-2">
                <VaultBadge faction={m.faction} />
              </span>
              <span class="absolute bottom-2 right-2 bg-black/80 px-2 py-0.5 text-[9px] font-mono rounded text-vault-brightGold border border-vault-gold/40">
                SCALE: {m.scale.split(' ')[0]}
              </span>
            </div>

            <!-- Details Area -->
            <div class="mt-3 flex-1 flex flex-col justify-between">
              <h3 class="font-cinzel text-sm font-bold text-gray-100 group-hover:text-vault-brightGold transition-colors truncate" title={m.name}>
                {m.name}
              </h3>
              <div class="flex items-center gap-2 my-2 text-[10px]">
                <span class="text-gray-400 font-mono uppercase">{m.unitType || 'TACTICAL'}</span>
                <span class="inline-flex items-center text-gray-600 leading-none">•</span>
                <span class="text-emerald-400 font-medium">In Stock</span>
              </div>
              <div class="pt-2.5 border-t border-[#232630] flex items-center justify-between">
                <span class="font-mono text-base font-bold text-vault-brightGold">${m.price.toFixed(2)}</span>
                <button
                  class="vault-btn-outline px-3 py-1.5 rounded text-[11px] font-semibold flex items-center gap-1.5"
                  onclick={(e) => quickAddToCart(e, m)}
                >
                  Add to Cart
                </button>
              </div>
            </div>
          </div>
        {/each}
      </div>
    {/if}
  </section>
  <!-- END: ProductCatalogArea -->
</main>
