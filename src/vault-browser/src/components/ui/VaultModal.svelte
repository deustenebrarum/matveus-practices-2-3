<script lang="ts">
  import type { Snippet } from 'svelte';

  interface Props {
    open: boolean;
    title?: string;
    maxWidth?: string;
    onclose: () => void;
    children?: Snippet;
  }

  let {
    open = false,
    title = '',
    maxWidth = 'max-w-4xl',
    onclose,
    children
  }: Props = $props();

  function handleKeydown(e: KeyboardEvent) {
    if (e.key === 'Escape' && open) {
      onclose();
    }
  }
</script>

<svelte:window onkeydown={handleKeydown} />

{#if open}
  <div
    class="fixed inset-0 z-50 flex items-center justify-center bg-black/85 backdrop-blur-sm p-4 transition-opacity duration-300"
    role="dialog"
    aria-modal="true"
    tabindex="-1"
    onclick={onclose}
    onkeydown={(e) => { if (e.key === 'Escape') onclose(); }}
  >
    <div
      class="vault-frame-ornate bg-[#12141c] w-full {maxWidth} rounded-sm p-6 relative max-h-[92vh] overflow-y-auto text-left shadow-2xl"
      role="document"
      tabindex="0"
      onclick={(e) => e.stopPropagation()}
      onkeydown={(e) => e.stopPropagation()}
    >
      <!-- Close Button -->
      <button
        aria-label="Close modal"
        class="absolute top-3 right-3 text-vault-gold hover:text-white transition-colors p-1 z-30"
        onclick={onclose}
      >
        <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path d="M6 18L18 6M6 6l12 12" stroke-linecap="round" stroke-linejoin="round" stroke-width="2"></path>
        </svg>
      </button>

      {#if title}
        <div class="text-center pb-4 mb-4 border-b border-[#3e3422]">
          <h2 class="font-cinzel text-xl sm:text-2xl font-bold tracking-widest text-[#e2be74] drop-shadow uppercase">
            {title}
          </h2>
        </div>
      {/if}

      {@render children?.()}
    </div>
  </div>
{/if}
