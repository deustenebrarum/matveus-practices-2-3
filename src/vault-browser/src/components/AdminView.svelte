<script lang="ts">
  import { onMount } from 'svelte';
  import type { InventoryItem } from '../types';
  import { fetchInventory, adjustInventory } from '../lib/api';
  import { ui } from '../lib/state/ui.svelte';
  import VaultCard from './ui/VaultCard.svelte';
  import VaultButton from './ui/VaultButton.svelte';

  let inventory = $state<InventoryItem[]>([]);
  let loading = $state<boolean>(true);
  let filterQuery = $state<string>('');

  onMount(async () => {
    await reloadInventory();
  });

  async function reloadInventory() {
    loading = true;
    try {
      inventory = await fetchInventory();
    } catch {
      ui.notify('Unable to contact Munitorum inventory mainframe.', 'crimson');
    } finally {
      loading = false;
    }
  }

  async function handleAdjust(miniatureId: string, delta: number) {
    try {
      const updated = await adjustInventory({ miniatureId, deltaQuantity: delta });
      inventory = inventory.map(i => i.id === miniatureId ? updated : i);
      ui.notify(`Updated stock for ${updated.miniatureName}: ${updated.availableStock} remaining.`, 'emerald');
    } catch {
      ui.notify('Stock adjustment failed.', 'crimson');
    }
  }

  const filteredItems = $derived(
    filterQuery.trim()
      ? inventory.filter(i => i.miniatureName.toLowerCase().includes(filterQuery.trim().toLowerCase()))
      : inventory
  );
</script>

<div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8 w-full">
  <!-- Breadcrumb -->
  <div class="flex items-center space-x-2 text-[11px] font-cinzel tracking-widest text-[#787265] uppercase mb-6">
    <button type="button" class="hover:text-vault-gold transition-colors cursor-pointer" onclick={() => ui.navigateTo('catalog')}>Catalog</button>
    <span>/</span>
    <span class="text-vault-gold">Munitorum Armory Logistics & Inventory</span>
  </div>

  <div class="flex flex-col sm:flex-row sm:items-center justify-between gap-4 pb-4 border-b border-[#38332b] mb-6">
    <div>
      <h1 class="font-cinzel font-bold text-2xl text-[#f2e6cb] tracking-wider uppercase">
        Munitorum Stock Depository
      </h1>
      <p class="text-xs text-[#8e897e] font-sans mt-1">
        Administrative inventory control, reserve allocation & stock adjustments (Marten / InventoryModule)
      </p>
    </div>
    <div class="flex items-center gap-3">
      <input
        class="bg-[#0c0e14] border border-[#5a482b] text-xs px-3 py-1.5 text-white placeholder-gray-500 focus:border-vault-gold focus:outline-none"
        placeholder="Filter stock by name..."
        bind:value={filterQuery}
      />
      <VaultButton
        variant="outline"
        size="sm"
        onclick={reloadInventory}
      >
        Sync
      </VaultButton>
    </div>
  </div>

  {#if loading}
    <div class="p-12 text-center text-vault-gold font-cinzel">
      Scanning Munitorum storage crypts...
    </div>
  {:else}
    <VaultCard brackets={true} class="p-4 overflow-x-auto">
      <table class="w-full text-left text-xs border-collapse">
        <thead>
          <tr class="border-b border-[#38332b] text-[11px] font-cinzel text-vault-brightGold uppercase tracking-wider">
            <th class="py-3 px-3">Relic / Miniature</th>
            <th class="py-3 px-3 text-center">Available Stock</th>
            <th class="py-3 px-3 text-center">Reserved</th>
            <th class="py-3 px-3">Last Sanctioned Update</th>
            <th class="py-3 px-3 text-right">Stock Adjustments</th>
          </tr>
        </thead>
        <tbody class="divide-y divide-[#232632]">
          {#each filteredItems as item (item.id)}
            <tr class="hover:bg-[#151822]/60 transition-colors">
              <td class="py-3.5 px-3 font-cinzel font-bold text-gray-200">
                {item.miniatureName}
              </td>
              <td class="py-3.5 px-3 text-center">
                <span class="font-mono font-bold text-sm {item.availableStock > 5 ? 'text-emerald-400' : item.availableStock > 0 ? 'text-amber-400' : 'text-red-500'}">
                  {item.availableStock}
                </span>
              </td>
              <td class="py-3.5 px-3 text-center font-mono text-gray-400">
                {item.reservedStock}
              </td>
              <td class="py-3.5 px-3 font-mono text-gray-500 text-[10px]">
                {new Date(item.lastUpdatedAt).toLocaleString()}
              </td>
              <td class="py-3.5 px-3 text-right">
                <div class="inline-flex items-center gap-1">
                  <button
                    type="button"
                    class="px-2 py-1 bg-[#1e1313] hover:bg-red-950 border border-red-900 text-red-300 rounded-none font-mono text-[11px] cursor-pointer"
                    onclick={() => handleAdjust(item.id, -5)}
                    title="Deduct 5"
                  >
                    -5
                  </button>
                  <button
                    type="button"
                    class="px-2 py-1 bg-[#1e1313] hover:bg-red-950 border border-red-900 text-red-300 rounded-none font-mono text-[11px] cursor-pointer"
                    onclick={() => handleAdjust(item.id, -1)}
                    title="Deduct 1"
                  >
                    -1
                  </button>
                  <button
                    type="button"
                    class="px-2 py-1 bg-[#131d16] hover:bg-emerald-950 border border-emerald-900 text-emerald-300 rounded-none font-mono text-[11px] cursor-pointer"
                    onclick={() => handleAdjust(item.id, 1)}
                    title="Add 1"
                  >
                    +1
                  </button>
                  <button
                    type="button"
                    class="px-2 py-1 bg-[#131d16] hover:bg-emerald-950 border border-emerald-900 text-emerald-300 rounded-none font-mono text-[11px] cursor-pointer"
                    onclick={() => handleAdjust(item.id, 5)}
                    title="Add 5"
                  >
                    +5
                  </button>
                </div>
              </td>
            </tr>
          {/each}
        </tbody>
      </table>
    </VaultCard>
  {/if}
</div>
