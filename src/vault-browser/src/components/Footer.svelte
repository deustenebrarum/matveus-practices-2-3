<script lang="ts">
  import { ui } from '../lib/state/ui.svelte';
</script>

<!-- BEGIN: SiteFooter matching design/account.html & design/index.html -->
<footer class="border-t border-[#38332b] bg-[#0c0d12] py-8 text-[#787265] text-xs mt-12" data-purpose="site-footer">
  <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 flex flex-col md:flex-row items-center justify-between gap-4">
    <div class="flex items-center space-x-3 text-center md:text-left">
      <svg class="w-8 h-8 text-vault-gold/60 shrink-0" fill="currentColor" viewBox="0 0 64 64">
        <path d="M32 6L28 14H18L24 22L16 28L26 30L20 40L30 36L32 44L34 36L44 40L38 30L48 28L40 22L46 14H36L32 6Z"></path>
      </svg>
      <div>
        <p class="font-cinzel text-[#dfcaa0] font-semibold tracking-wider uppercase">
          WARHAMMER VAULT • MINIATURE STORE
        </p>
        <p class="text-[10px] text-[#635d52] font-sans">
          Sanctioned tabletop miniatures, painting sets & wargear • Sector Imperialis Munitorum
        </p>
      </div>
    </div>
    <div class="flex items-center space-x-6 text-[11px] font-cinzel uppercase tracking-wider text-[#a7a296]">
      <button class="hover:text-vault-gold transition-colors" onclick={() => ui.navigateTo('catalog')}>Catalog</button>
      <button class="hover:text-vault-gold transition-colors" onclick={() => ui.navigateTo('account')}>Dossier</button>
      <button class="hover:text-vault-gold transition-colors" onclick={() => ui.navigateTo('admin')}>Inventory</button>
      <button class="hover:text-vault-gold transition-colors" onclick={() => ui.notify('Imperial astropathic relays are operational.', 'gold')}>Support</button>
    </div>
  </div>
</footer>
<!-- END: SiteFooter -->
