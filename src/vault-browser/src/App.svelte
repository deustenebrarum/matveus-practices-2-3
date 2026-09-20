<script lang="ts">
  import { ui } from './lib/state/ui.svelte';
  import { user } from './lib/state/user.svelte';
  import Header from './components/Header.svelte';
  import CatalogView from './components/CatalogView.svelte';
  import AccountView from './components/AccountView.svelte';
  import CheckoutView from './components/CheckoutView.svelte';
  import AdminView from './components/AdminView.svelte';
  import CartDrawer from './components/CartDrawer.svelte';
  import ProductDetailModal from './components/ProductDetailModal.svelte';
  import NotificationToast from './components/ui/NotificationToast.svelte';
  import Footer from './components/Footer.svelte';
</script>

<div class="min-h-screen flex flex-col justify-between bg-[#0a0b0e] text-[#dfcaa0]">
  <!-- Top Navigation Header -->
  <Header />

  <!-- Active View Routing -->
  <div class="flex-1 flex flex-col">
    {#if ui.activeView === 'catalog'}
      <CatalogView />
    {:else if ui.activeView === 'account'}
      <AccountView />
    {:else if ui.activeView === 'checkout'}
      <CheckoutView />
    {:else if ui.activeView === 'admin'}
      {#if user.isStaff}
        <AdminView />
      {:else}
        <div class="max-w-xl mx-auto my-20 p-8 border border-red-900/60 bg-[#120808] text-center">
          <div class="text-4xl text-red-500 mb-3">✠</div>
          <h2 class="font-cinzel text-lg font-bold text-red-400 uppercase tracking-widest mb-2">
            Inquisitorial Clearance Required
          </h2>
          <p class="text-xs text-gray-400 font-sans mb-6">
            Access to Munitorum logistics and stock depository is restricted to sanctioned staff personnel.
          </p>
          <button
            type="button"
            class="px-4 py-2 bg-[#2a1212] border border-red-700 text-red-200 font-cinzel text-xs uppercase hover:bg-red-900/40 cursor-pointer"
            onclick={() => ui.navigateTo('catalog')}
          >
            Return to Public Armory
          </button>
        </div>
      {/if}
    {/if}
  </div>

  <!-- Footer -->
  <Footer />

  <!-- Global Slide-out Cart Drawer -->
  <CartDrawer />

  <!-- Global Product Details Inspection Modal -->
  <ProductDetailModal />

  <!-- Global Notification Toast Alerts -->
  <NotificationToast />
</div>
