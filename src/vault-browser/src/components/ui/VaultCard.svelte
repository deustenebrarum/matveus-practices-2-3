<script lang="ts">
  import type { Snippet } from 'svelte';
  import CornerBrackets from './CornerBrackets.svelte';

  interface Props {
    brackets?: boolean;
    class?: string;
    onclick?: (e: MouseEvent) => void;
    children?: Snippet;
  }

  let {
    brackets = false,
    class: className = '',
    onclick,
    children
  }: Props = $props();
</script>

{#if brackets}
  <div
    class="vault-card p-4 {className}"
    role={onclick ? 'button' : undefined}
    tabindex={onclick ? 0 : undefined}
    {onclick}
    onkeydown={onclick ? (e) => { if (e.key === 'Enter' || e.key === ' ') onclick(e as unknown as MouseEvent); } : undefined}
  >
    <CornerBrackets>
      {@render children?.()}
    </CornerBrackets>
  </div>
{:else}
  <div
    class="vault-card p-4 {className}"
    role={onclick ? 'button' : undefined}
    tabindex={onclick ? 0 : undefined}
    {onclick}
    onkeydown={onclick ? (e) => { if (e.key === 'Enter' || e.key === ' ') onclick(e as unknown as MouseEvent); } : undefined}
  >
    {@render children?.()}
  </div>
{/if}
