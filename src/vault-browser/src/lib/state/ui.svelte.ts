import type { Miniature } from '../../types';

export type AppView = 'catalog' | 'account' | 'checkout' | 'admin';

export interface ToastNotification {
  id: number;
  message: string;
  type: 'gold' | 'crimson' | 'emerald';
}

function getPathForView(view: AppView): string {
  switch (view) {
    case 'account':
      return '/account';
    case 'checkout':
      return '/checkout';
    case 'admin':
      return '/admin';
    case 'catalog':
    default:
      return '/';
  }
}

function getViewForPath(pathname: string): AppView {
  const clean = pathname.toLowerCase().replace(/\/$/, '') || '/';
  if (clean === '/account') return 'account';
  if (clean === '/checkout') return 'checkout';
  if (clean === '/admin') return 'admin';
  return 'catalog';
}

class UiState {
  activeView = $state<AppView>(
    typeof window !== 'undefined' ? getViewForPath(window.location.pathname) : 'catalog'
  );
  cartDrawerOpen = $state<boolean>(false);
  activeModalProduct = $state<Miniature | null>(null);
  searchQuery = $state<string>('');
  selectedUniverse = $state<string>('');
  selectedFaction = $state<string>('');
  cartHeaderPulsing = $state<boolean>(false);
  notifications = $state<ToastNotification[]>([]);

  private toastId = 0;

  constructor() {
    if (typeof window !== 'undefined') {
      // Listen to browser Back / Forward navigation events
      window.addEventListener('popstate', (e) => {
        const view = e.state?.view || getViewForPath(window.location.pathname);
        this.activeView = view;
        if (this.activeModalProduct) this.closeProductModal();
        if (this.cartDrawerOpen) this.closeCart();
      });

      // Synchronize initial history state with current URL
      if (!window.history.state?.view) {
        window.history.replaceState(
          { view: this.activeView },
          '',
          window.location.pathname + window.location.search
        );
      }
    }
  }

  navigateTo(view: AppView, options?: { replace?: boolean; skipScroll?: boolean; search?: string }) {
    this.activeView = view;
    if (typeof window !== 'undefined') {
      const targetPath = getPathForView(view);
      const currentPath = window.location.pathname.replace(/\/$/, '') || '/';
      const normalizedTarget = targetPath.replace(/\/$/, '') || '/';
      const searchPart = options?.search !== undefined ? options.search : '';
      const fullTarget = targetPath + (searchPart ? (searchPart.startsWith('?') ? searchPart : '?' + searchPart) : '');

      if (currentPath !== normalizedTarget || (searchPart && window.location.search !== searchPart)) {
        if (options?.replace) {
          window.history.replaceState({ view }, '', fullTarget);
        } else {
          window.history.pushState({ view }, '', fullTarget);
        }
      }

      if (!options?.skipScroll) {
        window.scrollTo({ top: 0, behavior: 'smooth' });
      }
    }
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
      const qParam = query.trim() ? `?q=${encodeURIComponent(query.trim())}` : '';
      this.navigateTo('catalog', { search: qParam });
    }
  }

  setUniverse(universe: string) {
    this.selectedUniverse = universe;
    if (this.activeView !== 'catalog') {
      const uParam = universe.trim() ? `?universe=${encodeURIComponent(universe.trim())}` : '';
      this.navigateTo('catalog', { search: uParam });
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
