<script lang="ts">
  import { ui } from '../../lib/state/ui.svelte';
</script>

{#if ui.notifications.length > 0}
  <aside
    class="fixed bottom-6 right-6 z-50 flex flex-col gap-2.5 max-w-md w-full pointer-events-none px-4"
    aria-label="Notifications"
  >
    {#each ui.notifications as notif (notif.id)}
      <div
        class="pointer-events-auto bg-[#12141c] border-2 {notif.type === 'emerald' ? 'border-emerald-600' : notif.type === 'crimson' ? 'border-red-600' : 'border-vault-gold'} p-3.5 shadow-[0_0_20px_rgba(0,0,0,0.9)] flex items-center justify-between gap-3 animate-bounce"
        role="alert"
      >
        <div class="flex items-center gap-2.5 text-xs">
          <span class="text-sm">
            {notif.type === 'emerald' ? '⚡' : notif.type === 'crimson' ? '⚠' : '✠'}
          </span>
          <span class="font-cinzel text-gray-200 tracking-wide">
            {notif.message}
          </span>
        </div>
        <button
          class="text-gray-400 hover:text-white p-1 text-xs shrink-0"
          onclick={() => ui.dismissNotification(notif.id)}
        >
          ✕
        </button>
      </div>
    {/each}
  </aside>
{/if}
