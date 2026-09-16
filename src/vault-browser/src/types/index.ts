export interface WargearOption {
  type?: 'MeleeWargear' | 'RangedWargear' | 'SpecialEquipment' | string;
  name: string;
  pointsCost: number;
  weaponSkill?: string;
  strengthModifier?: number;
  damage?: number;
  rangeInches?: number;
  shots?: number;
  armorPenetration?: number;
  specialRules?: string;
  effectDescription?: string;
}

export interface Miniature {
  id: string;
  name: string;
  description: string;
  universe: string;
  faction: string;
  subfaction?: string;
  price: number;
  material: string;
  scale: string;
  baseSizeMm?: number;
  imageUrl: string;
  tags: string[];
  isFeatured: boolean;
  wargearOptions?: WargearOption[];
  points?: string;
  unitType?: string;
  stock?: number;
  inStock?: boolean;
}

export interface FactionCount {
  faction: string;
  count: number;
  universe: string;
}

export interface CartItem {
  id: string;
  name: string;
  price: number;
  quantity: number;
  imageUrl?: string;
  faction?: string;
  scale?: string;
  wargear?: string;
  isStarterSet?: boolean;
}

export interface CustomerInfo {
  fullName: string;
  email: string;
  phone: string;
  shippingAddress: string;
  city: string;
  postalCode?: string;
  courierService?: string;
}

export interface OrderItem {
  miniatureId: string;
  miniatureName: string;
  faction: string;
  isStarterSet: boolean;
  quantity: number;
  unitPrice: number;
}

export interface Order {
  id: string;
  orderNumber: string;
  customer: CustomerInfo;
  items: OrderItem[];
  subtotal: number;
  bundleDiscountAmount: number;
  promoDiscountAmount: number;
  appliedPromoCode?: string | null;
  totalAmount: number;
  status: string;
  createdAt: string;
}

export interface CreateOrderRequest {
  customer: CustomerInfo;
  items: {
    miniatureId: string;
    quantity: number;
  }[];
  promoCode?: string;
}

export interface InventoryItem {
  id: string;
  miniatureName: string;
  availableStock: number;
  reservedStock: number;
  lastUpdatedAt: string;
}

export interface AdjustInventoryRequest {
  miniatureId: string;
  deltaQuantity: number;
}

export interface CatalogFilterState {
  universe: string;
  factions: string[];
  materials: string[];
  scales: string[];
  minPrice: number;
  maxPrice: number;
  inStockOnly: boolean;
  searchQuery: string;
  sortBy: 'featured' | 'price_asc' | 'price_desc' | 'name';
}
