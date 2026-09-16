import type { CartItem } from '../../types';

const STORAGE_KEY = 'vault_cart_items';
const PROMO_STORAGE_KEY = 'vault_cart_promo';

function loadInitialItems(): CartItem[] {
  try {
    const raw = localStorage.getItem(STORAGE_KEY);
    if (raw) return JSON.parse(raw);
  } catch {
    // ignore
  }
  // Default items matching design/index.html
  return [
    {
      id: '11111111-1111-1111-1111-111111111111',
      name: 'Space Marine Tactical Squad',
      price: 39.90,
      quantity: 1,
      faction: 'Imperium',
      scale: '32mm Standard',
      imageUrl: 'https://lh3.googleusercontent.com/aida-public/AB6AXuCmix_3nOtb3lC5MIHp88FpjqxEsJ1lCbqHIHbh1yhPCKYEe7RRXPOVUi7N6HfkyHmKch2rC-zxhV39xfKkA0tyy-ZbFEs21MX5AvOnV9dg0hD62r9Nwx11V5YB34S-IY-7d6JEX6H3An4op2nvgLDrweVqkVgyVCvud4JRB36_yTC-FZj-uw38tu_TbUlYQoqB4rx38bIV7EA4jStj0BhD4Q5C0GUCL_-xsu600y_9pNYBJF2hSrA'
    },
    {
      id: '99999999-9999-9999-9999-999999999999',
      name: 'Citadel Base Paint Set',
      price: 60.00,
      quantity: 1,
      faction: 'Imperium',
      scale: 'Standard Pots',
      isStarterSet: true,
      imageUrl: 'https://lh3.googleusercontent.com/aida-public/AB6AXuC_ACLw9p31Jh1kwE_qI64Yc9_I70cB16fQq0swI-7Oz_GLOXiblkUrOxkDXZxO2jkSnqTEGzKERsfMRfOOp6sNAmKM6iJReJBZraSFnnf-F2-hS3Ik6emRbetv5M7zRHE8FyXWUZX4K8MDfDAnZDPufdsmlpv-POIK4ufkvmGqlgwu_A4nnAL5VVdZCv4JlQXlhyAdWr5Ql_VK5RA4KqfJxRcbGKapdNBPGC1q49SpGoJoVjwQJSU'
    }
  ];
}

class CartState {
  items = $state<CartItem[]>(loadInitialItems());
  promoCode = $state<string>('');
  promoDiscountRate = $state<number>(0);
  promoMessage = $state<string>('');

  count = $derived(
    this.items.reduce((acc, it) => acc + it.quantity, 0)
  );

  subtotal = $derived(
    Number(this.items.reduce((acc, it) => acc + it.price * it.quantity, 0).toFixed(2))
  );

  // Bundle discount is 15% when 2+ items or starter set is in cart, as shown in design/index.html
  hasBundleDiscount = $derived(
    this.items.length >= 2 || this.items.some(i => i.isStarterSet)
  );

  bundleDiscountRate = $derived(
    this.hasBundleDiscount ? 0.15 : 0.0
  );

  bundleDiscount = $derived(
    Number((this.subtotal * this.bundleDiscountRate).toFixed(2))
  );

  promoDiscount = $derived(
    Number((this.subtotal * this.promoDiscountRate).toFixed(2))
  );

  total = $derived(
    Number(Math.max(0, this.subtotal - this.bundleDiscount - this.promoDiscount).toFixed(2))
  );

  constructor() {
    try {
      const savedPromo = localStorage.getItem(PROMO_STORAGE_KEY);
      if (savedPromo) {
        this.applyPromo(savedPromo);
      }
    } catch {
      // ignore
    }
  }

  private persist() {
    try {
      localStorage.setItem(STORAGE_KEY, JSON.stringify(this.items));
      localStorage.setItem(PROMO_STORAGE_KEY, this.promoCode);
    } catch {
      // ignore
    }
  }

  addItem(item: {
    id: string;
    name: string;
    price: number;
    quantity?: number;
    imageUrl?: string;
    faction?: string;
    scale?: string;
    wargear?: string;
    isStarterSet?: boolean;
  }) {
    const qty = item.quantity && item.quantity > 0 ? item.quantity : 1;
    const existing = this.items.find(
      i => i.id === item.id && (i.wargear || '') === (item.wargear || '')
    );

    if (existing) {
      existing.quantity += qty;
    } else {
      this.items.push({
        id: item.id,
        name: item.name,
        price: item.price,
        quantity: qty,
        imageUrl: item.imageUrl,
        faction: item.faction,
        scale: item.scale,
        wargear: item.wargear,
        isStarterSet: item.isStarterSet
      });
    }
    this.persist();
  }

  removeItem(id: string, wargear?: string) {
    this.items = this.items.filter(
      i => !(i.id === id && (i.wargear || '') === (wargear || ''))
    );
    this.persist();
  }

  updateQuantity(id: string, delta: number, wargear?: string) {
    const item = this.items.find(
      i => i.id === id && (i.wargear || '') === (wargear || '')
    );
    if (!item) return;

    item.quantity += delta;
    if (item.quantity <= 0) {
      this.removeItem(id, wargear);
    } else {
      this.persist();
    }
  }

  applyPromo(code: string): { success: boolean; message: string } {
    const trimmed = (code || '').trim().toUpperCase();
    if (!trimmed) {
      this.promoCode = '';
      this.promoDiscountRate = 0;
      this.promoMessage = '';
      this.persist();
      return { success: false, message: 'Please enter a requisition code.' };
    }

    if (trimmed === 'TERRA10') {
      this.promoCode = trimmed;
      this.promoDiscountRate = 0.10;
      this.promoMessage = 'Imperial Sigil Recognized! +10% Requisition Discount.';
      this.persist();
      return { success: true, message: this.promoMessage };
    } else if (trimmed === 'EMPEROR20') {
      this.promoCode = trimmed;
      this.promoDiscountRate = 0.20;
      this.promoMessage = 'Blessed by the Golden Throne: +20% Tithe Relief.';
      this.persist();
      return { success: true, message: this.promoMessage };
    } else if (trimmed === 'WARP-TITHE-10') {
      this.promoCode = trimmed;
      this.promoDiscountRate = 0.10;
      this.promoMessage = 'Personal Voucher Active: 10% Citadel discount applied.';
      this.persist();
      return { success: true, message: this.promoMessage };
    } else {
      return { success: false, message: 'Invalid Imperial Seal or Expired Requisition Code.' };
    }
  }

  clearCart() {
    this.items = [];
    this.promoCode = '';
    this.promoDiscountRate = 0;
    this.promoMessage = '';
    this.persist();
  }
}

export const cart = new CartState();
