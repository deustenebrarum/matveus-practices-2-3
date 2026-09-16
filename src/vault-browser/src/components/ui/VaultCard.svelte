<script lang="ts">
  import type { Snippet } from 'svelte';

  interface Props {
    brackets?: boolean;
    class?: string;
    onclick?: (e: MouseEvent) => void;
    children?: Snippet;
    [key: string]: any;
  }

  let {
    brackets = false,
    class: className = '',
    onclick,
    children,
    ...restProps
  }: Props = $props();
</script>

<!-- svelte-ignore a11y_no_noninteractive_tabindex a11y_no_static_element_interactions -->
<div
  class="vault-card {brackets ? 'corner-brackets' : ''} {className}"
  role={onclick ? 'button' : undefined}
  tabindex={onclick ? 0 : undefined}
  {onclick}
  onkeydown={onclick ? (e) => { if (e.key === 'Enter' || e.key === ' ') onclick(e as unknown as MouseEvent); } : undefined}
  {...restProps}
>
  {#if brackets}
    <span class="bracket-tl" aria-hidden="true"></span>
    <span class="bracket-tr" aria-hidden="true"></span>
    <span class="bracket-bl" aria-hidden="true"></span>
    <span class="bracket-br" aria-hidden="true"></span>
  {/if}
  {@render children?.()}
</div>
