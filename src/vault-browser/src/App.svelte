<script lang="ts">
  import { ui } from './lib/state/ui.svelte';
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
      <AdminView />
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
