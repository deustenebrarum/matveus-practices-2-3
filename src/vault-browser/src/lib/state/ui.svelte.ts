import type { Miniature } from '../../types';

export type AppView = 'catalog' | 'account' | 'checkout' | 'admin';

export interface ToastNotification {
  id: number;
  message: string;
  type: 'gold' | 'crimson' | 'emerald';
}

class UiState {
  activeView = $state<AppView>('catalog');
  cartDrawerOpen = $state<boolean>(false);
  activeModalProduct = $state<Miniature | null>(null);
  searchQuery = $state<string>('');
  selectedUniverse = $state<string>('');
  selectedFaction = $state<string>('');
  cartHeaderPulsing = $state<boolean>(false);
  notifications = $state<ToastNotification[]>([]);

  private toastId = 0;

  navigateTo(view: AppView) {
    this.activeView = view;
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  openCart() {
    this.cartDrawerOpen = true;
    document.body.style.overflow = 'hidden';
  }

  closeCart() {
    this.cartDrawerOpen = false;
    document.body.style.overflow = 'auto';
  }

  toggleCart(forceState?: boolean) {
    if (forceState !== undefined) {
      if (forceState) this.openCart();
      else this.closeCart();
    } else {
      if (this.cartDrawerOpen) this.closeCart();
      else this.openCart();
    }
  }

  openProductModal(miniature: Miniature) {
    this.activeModalProduct = miniature;
    document.body.style.overflow = 'hidden';
  }

  closeProductModal() {
    this.activeModalProduct = null;
    document.body.style.overflow = 'auto';
  }

  setSearch(query: string) {
    this.searchQuery = query;
    if (this.activeView !== 'catalog') {
      this.activeView = 'catalog';
    }
  }

  setUniverse(universe: string) {
    this.selectedUniverse = universe;
    if (this.activeView !== 'catalog') {
      this.activeView = 'catalog';
    }
  }

  pulseCart() {
    this.cartHeaderPulsing = true;
    setTimeout(() => {
      this.cartHeaderPulsing = false;
    }, 400);
  }

  notify(message: string, type: 'gold' | 'crimson' | 'emerald' = 'gold') {
    const id = ++this.toastId;
    this.notifications.push({ id, message, type });
    setTimeout(() => {
      this.notifications = this.notifications.filter(n => n.id !== id);
    }, 4000);
  }

  dismissNotification(id: number) {
    this.notifications = this.notifications.filter(n => n.id !== id);
  }
}

export const ui = new UiState();
