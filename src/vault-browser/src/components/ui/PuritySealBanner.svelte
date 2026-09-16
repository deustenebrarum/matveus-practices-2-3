<script lang="ts">
  interface Props {
    title?: string;
    subtitle?: string;
    discountText?: string;
    class?: string;
  }

  let {
    title = 'Chapter Requisition Rite',
    subtitle = 'Bundle Set Discount Applied: -15%',
    discountText = '-15%',
    class: className = ''
  }: Props = $props();
</script>

<div
  class="relative bg-[#ebd9b2] text-[#2c2214] p-3 rounded-sm shadow-md border-l-4 border-[#8e1d1d] flex items-center justify-between {className}"
  role="alert"
>
  <div class="flex items-center gap-3">
    <!-- Wax Seal Icon -->
    <div
      class="w-9 h-9 rounded-full bg-[#8c1b1b] shadow flex items-center justify-center text-[#ffc6c6] font-serif font-black text-sm border-2 border-[#b53a3a] shrink-0 select-none"
      aria-hidden="true"
    >
      ✠
    </div>
    <div>
      <span class="font-cinzel text-xs font-black tracking-wide block uppercase text-[#5a1b1b]">
        {title}
      </span>
      <span class="text-[11px] font-semibold text-[#44331e]">
        {subtitle}
      </span>
    </div>
  </div>
  <span class="font-mono text-xs font-bold text-[#8c1b1b] shrink-0">
    {discountText}
  </span>
</div>
