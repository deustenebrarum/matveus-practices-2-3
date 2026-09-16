<script lang="ts">
  import type { Snippet } from 'svelte';

  interface Props {
    faction?: string;
    class?: string;
    children?: Snippet;
  }

  let {
    faction = '',
    class: className = '',
    children
  }: Props = $props();

  const normalized = $derived(faction.toLowerCase());

  const badgeStyle = $derived(
    normalized.includes('chaos')
      ? 'border-red-900 text-red-300 bg-red-950/40'
      : normalized.includes('necron') || normalized.includes('xenos')
      ? 'border-emerald-800 text-emerald-300 bg-emerald-950/30'
      : normalized.includes('ork')
      ? 'border-amber-900 text-amber-300 bg-amber-950/30'
      : normalized.includes('order') || normalized.includes('sigmar')
      ? 'border-sky-800 text-sky-200 bg-sky-950/30'
      : 'border-[#4a3e29] text-[#d1b87d] bg-[#12141a]/90'
  );
</script>

<span class="vault-badge {badgeStyle} {className}">
  {#if normalized.includes('chaos')}
    <svg class="w-3.5 h-3.5 inline-block text-red-400 shrink-0" viewBox="0 0 24 24" fill="currentColor">
      <path d="M12 1l2.2 4-1.2.7V8l2.5-1.5.8 1.4-2.5 1.5 2 2 1.3-.8.8 1.3-2.1 1.2h2.5v1.6H18l2.1 1.2-.8 1.3-1.3-.8-2 2 2.5 1.5-.8 1.4-2.5-1.5v2.3l1.2.7-2.2 4-2.2-4 1.2-.7V18l-2.5 1.5-.8-1.4 2.5-1.5-2-2-1.3.8-.8-1.3 2.1-1.2H6.5v-1.6H9l-2.1-1.2.8-1.3 1.3.8 2-2-2.5-1.5.8-1.4L11 8V5.7l-1.2-.7L12 1zm0 8.5a2.5 2.5 0 100 5 2.5 2.5 0 000-5z"></path>
    </svg>
  {:else if normalized.includes('necron')}
    <svg class="w-3.5 h-3.5 inline-block text-emerald-400 shrink-0" viewBox="0 0 24 24" fill="currentColor">
      <path d="M12 2a4 4 0 00-4 4c0 1.8 1.2 3.3 2.8 3.8L9 11.5H6v2h3.2l-.7 1.5H5v2h2.3L6 20h2.2l1.2-3h5.2l1.2 3h2.2l-1.3-3H19v-2h-3.5l-.7-1.5H18v-2h-3l-1.8-1.7C14.8 9.3 16 7.8 16 6a4 4 0 00-4-4zm0 2.2a1.8 1.8 0 110 3.6 1.8 1.8 0 010-3.6zm-1.8 7.3h3.6l1 2h-5.6l1-2z"></path>
    </svg>
  {:else if normalized.includes('ork')}
    <svg class="w-3.5 h-3.5 inline-block text-amber-400 shrink-0" viewBox="0 0 24 24" fill="currentColor">
      <path d="M4 3l3 4h10l3-4-1 6 2 3-3 2v6l-3-2-3 2-3-2-3 2v-6l-3-2 2-3-1-6zm4 6v2h2V9H8zm6 0v2h2V9h-2zm-5 4h6v2H9v-2z"></path>
    </svg>
  {:else}
    <svg class="w-3.5 h-3.5 inline-block text-vault-brightGold shrink-0" viewBox="0 0 24 24" fill="currentColor">
      <path d="M12 2l2 2.5h3.5l-1.5 3 4-1-1 4.5 3 .5-3 2.5 1.5 3.5-3.5-1-1 3.5-4.5-3.5-4.5 3.5-1-3.5-3.5 1 1.5-3.5-3-2.5 3-.5-1-4.5 4 1-1.5-3H10L12 2zm0 5a2 2 0 100 4 2 2 0 000-4z"></path>
    </svg>
  {/if}
  {#if children}
    {@render children()}
  {:else}
    {faction}
  {/if}
</span>
