<script lang="ts">
  import type { Snippet } from 'svelte';

  interface Props {
    class?: string;
    children?: Snippet;
  }

  let { class: className = '', children }: Props = $props();
</script>

<div class="corner-brackets relative {className}">
  <span class="bracket-tl" aria-hidden="true"></span>
  <span class="bracket-tr" aria-hidden="true"></span>
  <span class="bracket-bl" aria-hidden="true"></span>
  <span class="bracket-br" aria-hidden="true"></span>
  {@render children?.()}
</div>
