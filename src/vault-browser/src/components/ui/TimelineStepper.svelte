<script lang="ts">
  interface Step {
    label: string;
    subtext?: string;
  }

  interface Props {
    currentStep?: number; // 1 to 4
    steps?: Step[];
    class?: string;
  }

  let {
    currentStep = 3,
    steps = [
      { label: '1. Order Processing', subtext: 'Sep 15, 10:20' },
      { label: '2. Forge Packaging', subtext: 'Sep 15, 16:45' },
      { label: '3. In Transit (CDEK)', subtext: 'Tracking: CDEK-40K-8492091' },
      { label: '4. Ready for Delivery', subtext: 'Expected Sep 18' }
    ],
    class: className = ''
  }: Props = $props();

  const progressPercent = $derived(
    currentStep <= 1
      ? '10%'
      : currentStep === 2
      ? '38%'
      : currentStep === 3
      ? '68%'
      : '100%'
  );
</script>

<div class="w-full pt-2 {className}">
  <div class="relative">
    <!-- Progress Bar Background -->
    <div class="h-1.5 w-full bg-[#1b1e28] absolute top-3.5 -translate-y-1/2 z-0"></div>
    <!-- Active Progress Bar -->
    <div
      class="h-1.5 bg-gradient-to-r from-vault-gold via-amber-400 to-amber-300 absolute top-3.5 -translate-y-1/2 z-0 shadow-[0_0_10px_rgba(197,155,67,0.7)] transition-all duration-500"
      style="width: {progressPercent};"
    ></div>
    <!-- Steps -->
    <div class="relative z-10 flex justify-between">
      {#each steps as step, idx}
        {@const stepNum = idx + 1}
        {@const isCompleted = stepNum < currentStep}
        {@const isActive = stepNum === currentStep}
        <div class="flex flex-col items-center">
          {#if isCompleted}
            <div class="w-7 h-7 bg-vault-gold border-2 border-vault-gold text-[#0a0b0e] flex items-center justify-center text-xs font-bold shadow-md">
              ✓
            </div>
            <span class="font-cinzel text-[11px] text-[#f2e6cb] mt-2 font-semibold text-center">
              {step.label}
            </span>
            {#if step.subtext}
              <span class="text-[9px] font-mono text-[#787265] mt-0.5">
                {step.subtext}
              </span>
            {/if}
          {:else if isActive}
            <div class="w-7 h-7 bg-[#12141a] border-2 border-vault-gold text-vault-gold flex items-center justify-center text-xs font-bold animate-pulse shadow-[0_0_12px_rgba(197,155,67,0.85)]">
              ⚡
            </div>
            <span class="font-cinzel text-[11px] text-vault-gold mt-2 font-bold tracking-wide text-center">
              {step.label}
            </span>
            {#if step.subtext}
              <span class="text-[9px] font-mono text-vault-gold/90 mt-0.5">
                {step.subtext}
              </span>
            {/if}
          {:else}
            <div class="w-7 h-7 bg-[#14161f] border-2 border-[#38332b] text-[#555047] flex items-center justify-center text-xs">
              {stepNum}
            </div>
            <span class="font-cinzel text-[11px] text-[#736d62] mt-2 text-center">
              {step.label}
            </span>
            {#if step.subtext}
              <span class="text-[9px] font-mono text-[#4d473d] mt-0.5">
                {step.subtext}
              </span>
            {/if}
          {/if}
        </div>
      {/each}
    </div>
  </div>
</div>
