<script lang="ts">
  import type { Snippet } from 'svelte';

  interface Props {
    variant?: 'gold' | 'outline' | 'crimson' | 'dark';
    size?: 'sm' | 'md' | 'lg';
    type?: 'button' | 'submit' | 'reset';
    disabled?: boolean;
    class?: string;
    title?: string;
    onclick?: (e: MouseEvent) => void;
    children?: Snippet;
  }

  let {
    variant = 'gold',
    size = 'md',
    type = 'button',
    disabled = false,
    class: className = '',
    title = '',
    onclick,
    children
  }: Props = $props();

  const variantClass = $derived(
    variant === 'gold'
      ? 'vault-btn-gold text-black font-bold'
      : variant === 'outline'
      ? 'vault-btn-outline'
      : variant === 'crimson'
      ? 'bg-[#251010] hover:bg-[#3d1515] border border-[#8a1c1c] text-red-300 font-cinzel hover:border-red-500'
      : 'bg-[#161822] hover:bg-[#25221c] border border-[#4a3e28] text-xs font-cinzel uppercase text-[#c59b43]'
  );

  const sizeClass = $derived(
    size === 'sm'
      ? 'px-2.5 py-1 text-[10px] tracking-wider'
      : size === 'lg'
      ? 'px-6 py-3 text-sm tracking-widest'
      : 'px-4 py-2 text-xs tracking-wider'
  );
</script>

<button
  {type}
  {disabled}
  {title}
  class="{variantClass} {sizeClass} {className} transition-all select-none inline-flex items-center justify-center gap-1.5"
  {onclick}
>
  {@render children?.()}
</button>
