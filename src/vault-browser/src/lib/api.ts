import type {
  Miniature,
  FactionCount,
  CreateOrderRequest,
  Order,
  InventoryItem,
  AdjustInventoryRequest
} from '../types';

// Fallback seed catalog matching design/index.html + SeedDataService
export const SEED_MINIATURES: Miniature[] = [
  {
    id: '11111111-1111-1111-1111-111111111111',
    name: 'Space Marine Captain',
    description: 'A stalwart veteran of countless planetary purges, the Primaris Captain leads from the battlefront with absolute resolve. Armed with ancient chapter relics and unyielding faith in the Golden Throne.',
    universe: 'Warhammer 40,000',
    faction: 'Imperium',
    subfaction: 'Adeptus Astartes',
    price: 42.00,
    material: 'Citadel Plastic',
    scale: '32mm Standard',
    baseSizeMm: 40,
    imageUrl: 'https://lh3.googleusercontent.com/aida-public/AB6AXuAvrktdY3q6xdAIuPjE0b34GVMYg9lwLvNdBQNC7loqq9AxB3EaWDhCfqORxN01S6iAY-hceugVnndPPZOUvnP0CWBtJmgk3XfRWZsQpjX9RzGUC37ltKBzZVcLI6IHE8CobjltV46D0D-o4N1HcXbU3IpPndAKvMkVE3KKqlId-z6mc5dMiLBKICoAmMJzKnJg3Q2VFGDKmiaU9CtXY9OlC2-7BVAhmLLK2QJDHrVlqJp_kcDwquU',
    tags: ['Hero', 'Imperium', 'Adeptus Astartes'],
    isFeatured: true,
    points: '85 pts',
    unitType: 'HERO UNIT',
    stock: 5,
    inStock: true,
    wargearOptions: [
      { name: 'Master-Crafted Power Sword & Relic Shield', pointsCost: 0 },
      { name: 'Heavy Bolt Pistol & Master-crafted Auto Bolt Rifle', pointsCost: 0 },
      { name: 'Thunder Hammer & Plasma Pistol (+10 pts)', pointsCost: 10 },
      { name: 'Power Fist & Plasma Pistol', pointsCost: 5 }
    ]
  },
  {
    id: '22222222-2222-2222-2222-222222222222',
    name: 'Necron Overlord',
    description: 'Ancient android warlords ruling dynasties for millions of years with supreme contempt for mortal flesh. Equipped with high-phase weaponry and hyperdense necrodermis.',
    universe: 'Warhammer 40,000',
    faction: 'Xenos',
    subfaction: 'Necrons',
    price: 48.00,
    material: 'Citadel Plastic',
    scale: '32mm Standard',
    baseSizeMm: 40,
    imageUrl: 'https://lh3.googleusercontent.com/aida-public/AB6AXuB3D2tP4c1JIFlL4EXEGLvpLtj__xnMxy7dWwni0WjDCbsOrC9EUH8E334jajk4Q1ZiOEoY5XfyP0whf2zMQ6cy0FO1EheyGMPC4x9Zo91jnP2NL6NpiZdoI84R6JioRAeD-k-oVFuwuq1vspyActu8VycUaYX7hmVHKkW5xYnJaWCV-fkEFTOUbKESBADOGEgAHUsWgdD87NJSxTgzD6RqfP_7a_wcwONJN8o8XfgLPgevue0J9jE',
    tags: ['Hero', 'Xenos', 'Necrons', 'Warlord'],
    isFeatured: true,
    points: '90 pts',
    unitType: 'WARLORD',
    stock: 3,
    inStock: true,
    wargearOptions: [
      { name: 'Hyperphase Glaive & Tachyon Arrow', pointsCost: 0 },
      { name: 'Staff of Light & Resurrection Orb', pointsCost: 15 },
      { name: 'Voidscythe & Phase Shifter', pointsCost: 20 }
    ]
  },
  {
    id: '33333333-3333-3333-3333-333333333333',
    name: 'Chaos Space Marine Champion',
    description: 'Chosen champions of the Ruinous Powers, clad in baroque relic armor and hardened through millennia in the Eye of Terror.',
    universe: 'Warhammer 40,000',
    faction: 'Chaos',
    subfaction: 'Heretic Astartes',
    price: 36.00,
    material: 'Citadel Plastic',
    scale: '32mm Standard',
    baseSizeMm: 32,
    imageUrl: 'https://lh3.googleusercontent.com/aida-public/AB6AXuCDb8lNEnNNFviDCFAaqTRGy5loL1XcOODtuOB2Zft7-MUYIWHPDin4BQMxFKkf3JQUAwbD1V_j_NTbu0d2bPn4zHsRbCBFReaBapiir31deS7XdRGYeY7Y_FT7DcLghOUKoNpYDWYACExmQIg3w6KIq-PHGo-dBVvOKe5rbq_gTyL2BAh2jRz1DihPBHsuUIEPRTkCDoy8lMYcKjuxpbZhadioRHmM0yuNHeTYJYAUkIY17k9I72g',
    tags: ['Infantry', 'Chaos', 'Heretic Astartes'],
    isFeatured: true,
    points: '75 pts',
    unitType: 'INFANTRY',
    stock: 8,
    inStock: true,
    wargearOptions: [
      { name: 'Daemon Blade & Plasma Pistol', pointsCost: 5 },
      { name: 'Boltgun & Chainsword', pointsCost: 0 },
      { name: 'Power Fist & Combi-Bolter', pointsCost: 10 }
    ]
  },
  {
    id: '44444444-4444-4444-4444-444444444444',
    name: 'Ork Warboss',
    description: 'The largest, meanest, and greenest brute leading the WAAAGH! armed with a customized power klaw and snarling attack squig.',
    universe: 'Warhammer 40,000',
    faction: 'Xenos',
    subfaction: 'Orks',
    price: 54.00,
    material: 'Citadel Plastic',
    scale: '40mm Terminator',
    baseSizeMm: 40,
    imageUrl: 'https://lh3.googleusercontent.com/aida-public/AB6AXuBgz4ozWqpB6uVkgGfyciahXABrWnLslEEpHz7ux9gVDUHSs9HA60ZoC-zOlv4HJegmJXqqCr7uKgJkPxgUwGiHHOMu79wg-0wucADlwEi3Uw4EkAAaeBKOZ9Td9ELS2APA1UDtr_L_0_QE0u8o70FZnp-_lYF8SjLmtg-8_Ga19_rn2Ra-YVoAo3uSdVQMP43uLfo4kcVV190qqB55LTLzccrWdmlIgo67FBclQTZ-ziixmQS2UC8',
    tags: ['Hero', 'Xenos', 'Orks', 'Leader'],
    isFeatured: true,
    points: '110 pts',
    unitType: 'LEADER',
    stock: 2,
    inStock: true,
    wargearOptions: [
      { name: 'Kombi-Shoota & Power Klaw', pointsCost: 0 },
      { name: 'Big Choppa & Attack Squig', pointsCost: 5 }
    ]
  },
  {
    id: '55555555-5555-5555-5555-555555555555',
    name: 'Chaos Havoc Specialist',
    description: 'Heavy weapons specialists wielding reaper chaincannons, lascannons, and missile launchers to annihilate imperial armor.',
    universe: 'Warhammer 40,000',
    faction: 'Chaos',
    subfaction: 'Heretic Astartes',
    price: 42.00,
    material: 'Citadel Plastic',
    scale: '32mm Standard',
    baseSizeMm: 40,
    imageUrl: 'https://lh3.googleusercontent.com/aida-public/AB6AXuCZpvbZTTNFLuVr53faNe6zwmgdTCmnhcSxP7lKmIMFYuLk7BP9eZQ25ZTEIcaO4IBuRCb3FYEiQu_uRvhkps1_N8ww0Ouvd41yzvZsNR9hoX_WJNWPVWk8s7ix3wsUss1-PO84fAS0SdetchRbBx0DDdHi68taWfAGH-2x_bvohwnqKN3fBg4XtANakB2uf4EbVjCrb_TcsvjHyVsWEyl_25GayLeKtgH8cGLv8P9EnPDSJmSzyfY',
    tags: ['Infantry', 'Chaos', 'Heretic Astartes', 'FireSupport'],
    isFeatured: false,
    points: '80 pts',
    unitType: 'FIRE SUPPORT',
    stock: 6,
    inStock: true,
    wargearOptions: [
      { name: 'Reaper Chaincannon', pointsCost: 10 },
      { name: 'Lascannon Array', pointsCost: 15 },
      { name: 'Autocannon', pointsCost: 0 }
    ]
  },
  {
    id: '66666666-6666-6666-6666-666666666666',
    name: 'Chaos Terminator Lord',
    description: 'Clad in massive relic Cataphractii plate and bristling with mutated spikes and warp talons.',
    universe: 'Warhammer 40,000',
    faction: 'Chaos',
    subfaction: 'Heretic Astartes',
    price: 49.00,
    material: 'Citadel Plastic',
    scale: '40mm Terminator',
    baseSizeMm: 40,
    imageUrl: 'https://lh3.googleusercontent.com/aida-public/AB6AXuB_9V-tJHe4D7WwYeTv4uBYT2zlFmzUXfbqbWb7JZ69tmXplaTPPTky-_4s4YSp3WRi5DcsbSK4MJBnKRjBMGGeQ0dDkO_YAPAWsewk_miuZ3UF7Gxp7Albd2djKjeUm2rHgprCU4YQgNuNwOAWe8uF1jHnhfCI57MGpD_Atnl7LZd7cFTsKFuSD4vNhZCgCilVJDW8oYbHE9jUl-NqThkXGrRZfjLTq5ZZtQO97ly3XXE6C038Vfs',
    tags: ['Hero', 'Chaos', 'Elite'],
    isFeatured: false,
    points: '105 pts',
    unitType: 'ELITE',
    stock: 4,
    inStock: true,
    wargearOptions: [
      { name: 'Chainfist & Combi-Melta', pointsCost: 10 },
      { name: 'Lightning Claws Pair', pointsCost: 5 }
    ]
  },
  {
    id: '77777777-7777-7777-7777-777777777777',
    name: 'Custodes Guard',
    description: 'The Golden Legion, literal bodyguards of the Master of Mankind. Genetically sculpted demigods of unparalleled martial prowess.',
    universe: 'Warhammer 40,000',
    faction: 'Imperium',
    subfaction: 'Talons of the Emperor',
    price: 58.00,
    material: 'Citadel Plastic',
    scale: '40mm Terminator',
    baseSizeMm: 40,
    imageUrl: 'https://lh3.googleusercontent.com/aida-public/AB6AXuBKRaab5L7LIKbwJDE_7SrDtmjNt79FlTVGK5MV13rEGCq6RdA0qnS1HP_MMUmeKERtCkc-BKSn5_u6GGQd2qfRzW7wqJlhejzX5FWMntwjnQ720Bhw96AVewLj3YnNAECLzzzfytisYy1zotoE5vHdkGxxSECkWpoV1o9VXUFW1tyxyUIXjCAGLNsi18VoG5JN5epJhPzy7ADG3mhFJeaCuoZvQwymncJyVCbRu6ZYP7HuejiZ5xI',
    tags: ['Infantry', 'Imperium', 'Bodyguard'],
    isFeatured: true,
    points: '135 pts',
    unitType: 'BODYGUARD',
    stock: 3,
    inStock: true,
    wargearOptions: [
      { name: 'Guardian Spear with Integrated Bolter', pointsCost: 0 },
      { name: 'Sentinel Blade & Praesidium Shield', pointsCost: 5 }
    ]
  },
  {
    id: '88888888-8888-8888-8888-888888888888',
    name: 'Stormcast Liberators Squad',
    description: 'Sigmar’s celestial crusaders reforged in celestial lightning, waging relentless war across the Mortal Realms.',
    universe: 'Age of Sigmar',
    faction: 'Order',
    subfaction: 'Stormcast Eternals',
    price: 45.00,
    material: 'Citadel Plastic',
    scale: '32mm Standard',
    baseSizeMm: 40,
    imageUrl: 'https://lh3.googleusercontent.com/aida-public/AB6AXuBxNa6rIn3rpi0X7fTquTnnXpbhfPRge9JvYu_fCw53xpgUPoPtC-mXdGzGDqRnaS9xMBMNJ_uARoeM1VLTyIW1nLJeYXhEKwaNxQVyHGAIXbZsnEI1tIkvoiN2YM-CHrUyGiW6WqAs_VP9Um4EMomL1LWe6w-GFByor_oAnRelOGM8VqQ6svV6xKrrm9uCFF5ORH01zPCCBkKvFm8g3LtE0fChSacsyhNR2PD2St6ApDLZWLnCj-o',
    tags: ['Infantry', 'Order', 'Stormcast'],
    isFeatured: false,
    points: '110 pts',
    unitType: 'INFANTRY',
    stock: 6,
    inStock: true,
    wargearOptions: [
      { name: 'Warhammer & Sigmarite Shield', pointsCost: 0 },
      { name: 'Paired War-blades', pointsCost: 0 }
    ]
  },
  {
    id: '99999999-9999-9999-9999-999999999999',
    name: 'Citadel Base Paint Set',
    description: 'Essential hobby paint kit containing 12 authentic Citadel acrylic paints, shade washes, and starter brushes.',
    universe: 'Warhammer 40,000',
    faction: 'Imperium',
    subfaction: 'Citadel Supplies',
    price: 60.00,
    material: 'Acrylic Paint',
    scale: 'Standard Pots',
    baseSizeMm: 0,
    imageUrl: 'https://lh3.googleusercontent.com/aida-public/AB6AXuC_ACLw9p31Jh1kwE_qI64Yc9_I70cB16fQq0swI-7Oz_GLOXiblkUrOxkDXZxO2jkSnqTEGzKERsfMRfOOp6sNAmKM6iJReJBZraSFnnf-F2-hS3Ik6emRbetv5M7zRHE8FyXWUZX4K8MDfDAnZDPufdsmlpv-POIK4ufkvmGqlgwu_A4nnAL5VVdZCv4JlQXlhyAdWr5Ql_VK5RA4KqfJxRcbGKapdNBPGC1q49SpGoJoVjwQJSU',
    tags: ['Paints', 'StarterSet', 'Supplies'],
    isFeatured: false,
    points: 'Hobby',
    unitType: 'SUPPLIES',
    stock: 12,
    inStock: true,
    wargearOptions: [
      { name: '12 Citadel Acrylics & Synthetic Brush', pointsCost: 0 }
    ]
  },
  {
    id: '88888888-8888-8888-8888-888888888889',
    name: 'Ork Nob Veteran',
    description: 'Veteran nobz stomping ahead of the boyz with custom kustom big choppas and ‘eavy armor plates, yelling battle-cries across the trench-lines.',
    universe: 'Warhammer 40,000',
    faction: 'Xenos',
    subfaction: 'Orks',
    price: 38.00,
    material: 'Citadel Plastic',
    scale: '32mm Standard',
    baseSizeMm: 32,
    imageUrl: 'https://lh3.googleusercontent.com/aida-public/AB6AXuAnrlmIFE2mo5sBm48HGhY2LA-6ysyzEY9DC-BJrrFYulcQskW4qOXKOA6lGRuIwXJ929YZzscAqJ1WsL7MG23sjQyFAjyWmb-EFN31njtBSQTAOxAFqywvCdAyJoeofVIyCJ3v7_pq3yVPNLtrkXC94wPahcgO91u8XGetrHQlAt13TGBRN-GQUi9uB9AYLl8B04qX0fRQR6bLxSnywo3RMIfG9ailStFjsQAH61flGsJOEA-TXpg',
    tags: ['Infantry', 'Xenos', 'Orks'],
    isFeatured: false,
    points: '65 pts',
    unitType: 'INFANTRY',
    stock: 7,
    inStock: true,
    wargearOptions: [
      { name: 'Custom Big Choppa', pointsCost: 0 },
      { name: 'Power Klaw & Slugga', pointsCost: 5 }
    ]
  }
];

// Initial local storage keys
const LOCAL_STORAGE_ORDERS_KEY = 'vault_orders_history';
const LOCAL_STORAGE_INVENTORY_KEY = 'vault_inventory_stock';

function getLocalOrders(): Order[] {
  try {
    const raw = localStorage.getItem(LOCAL_STORAGE_ORDERS_KEY);
    if (raw) return JSON.parse(raw);
  } catch {
    // ignore
  }
  return [
    {
      id: 'a1000000-0000-0000-0000-000000000001',
      orderNumber: 'WH-84920',
      customer: {
        fullName: 'Master Valtor (Vane Malcor)',
        email: 'inquisitor.malcor@imperium.vault',
        phone: '+7 (999) 40K-1984',
        shippingAddress: 'Tverskaya St, 12, bld. 2',
        city: 'Moscow',
        courierService: 'CDEK Express'
      },
      items: [
        {
          miniatureId: '11111111-1111-1111-1111-111111111111',
          miniatureName: 'Space Marine Captain',
          faction: 'Imperium',
          isStarterSet: false,
          quantity: 1,
          unitPrice: 42.00
        },
        {
          miniatureId: '99999999-9999-9999-9999-999999999999',
          miniatureName: 'Citadel Base Paint Set',
          faction: 'Imperium',
          isStarterSet: true,
          quantity: 1,
          unitPrice: 60.00
        }
      ],
      subtotal: 102.00,
      bundleDiscountAmount: 15.30,
      promoDiscountAmount: 0,
      appliedPromoCode: 'BUNDLE15',
      totalAmount: 86.70,
      status: 'In Transit',
      createdAt: '2026-09-15T10:20:00Z'
    },
    {
      id: 'a2000000-0000-0000-0000-000000000002',
      orderNumber: 'WH-73109',
      customer: {
        fullName: 'Master Valtor',
        email: 'inquisitor.malcor@imperium.vault',
        phone: '+7 (999) 40K-1984',
        shippingAddress: 'Nevsky Ave, 45',
        city: 'Saint Petersburg',
        courierService: 'Direct Delivery'
      },
      items: [
        {
          miniatureId: '22222222-2222-2222-2222-222222222222',
          miniatureName: 'Necron Overlord with Hyperphase Blade',
          faction: 'Xenos',
          isStarterSet: false,
          quantity: 1,
          unitPrice: 48.00
        }
      ],
      subtotal: 48.00,
      bundleDiscountAmount: 0,
      promoDiscountAmount: 0,
      appliedPromoCode: null,
      totalAmount: 48.00,
      status: 'Delivered',
      createdAt: '2026-05-28T14:15:00Z'
    }
  ];
}

function saveLocalOrders(orders: Order[]): void {
  try {
    localStorage.setItem(LOCAL_STORAGE_ORDERS_KEY, JSON.stringify(orders));
  } catch {
    // ignore
  }
}

function getLocalInventory(): InventoryItem[] {
  try {
    const raw = localStorage.getItem(LOCAL_STORAGE_INVENTORY_KEY);
    if (raw) return JSON.parse(raw);
  } catch {
    // ignore
  }
  return SEED_MINIATURES.map(m => ({
    id: m.id,
    miniatureName: m.name,
    availableStock: m.stock ?? 10,
    reservedStock: 0,
    lastUpdatedAt: new Date().toISOString()
  }));
}

function saveLocalInventory(items: InventoryItem[]): void {
  try {
    localStorage.setItem(LOCAL_STORAGE_INVENTORY_KEY, JSON.stringify(items));
  } catch {
    // ignore
  }
}

export async function fetchMiniatures(): Promise<Miniature[]> {
  try {
    const res = await fetch('/api/catalog', { method: 'GET', credentials: 'omit' });
    if (res.ok) {
      const data = await res.json();
      const serverItems: Miniature[] = Array.isArray(data) ? data : data.items || [];
      if (serverItems.length > 0) {
        // Merge or return server items with normalized USD prices and images
        return serverItems.map(s => {
          const fallback = SEED_MINIATURES.find(seed => seed.id === s.id || seed.name.toLowerCase().includes(s.name.toLowerCase().split(' ')[0]));
          return {
            ...s,
            price: s.price > 500 ? Number((s.price / 100).toFixed(2)) : s.price,
            imageUrl: s.imageUrl?.startsWith('http') ? s.imageUrl : (fallback?.imageUrl || SEED_MINIATURES[0].imageUrl),
            stock: s.stock ?? 10,
            inStock: true,
            unitType: s.tags?.includes('Hero') ? 'HERO UNIT' : s.tags?.includes('StarterSet') ? 'STARTER SET' : 'INFANTRY'
          };
        });
      }
    }
  } catch {
    // Backend offline; use seed items
  }
  return SEED_MINIATURES;
}

export async function fetchMiniatureById(id: string): Promise<Miniature | null> {
  try {
    const res = await fetch(`/api/catalog/${id}`);
    if (res.ok) {
      return await res.json();
    }
  } catch {
    // fallback
  }
  return SEED_MINIATURES.find(m => m.id === id) || null;
}

export async function fetchFactions(): Promise<FactionCount[]> {
  try {
    const res = await fetch('/api/catalog/factions');
    if (res.ok) {
      return await res.json();
    }
  } catch {
    // fallback
  }
  // Compute from seed
  const map = new Map<string, { count: number; universe: string }>();
  for (const m of SEED_MINIATURES) {
    const existing = map.get(m.faction) || { count: 0, universe: m.universe };
    existing.count++;
    map.set(m.faction, existing);
  }
  return Array.from(map.entries()).map(([faction, val]) => ({
    faction,
    count: val.count,
    universe: val.universe
  }));
}

export async function createOrder(request: CreateOrderRequest): Promise<Order> {
  try {
    const res = await fetch('/api/orders', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(request)
    });
    if (res.ok) {
      const order = await res.json();
      // Also cache in local orders
      const existing = getLocalOrders();
      saveLocalOrders([order, ...existing]);
      return order;
    }
  } catch {
    // Backend unavailable, generate local order
  }

  // Fallback local order creation
  const seedMap = new Map(SEED_MINIATURES.map(m => [m.id, m]));
  const orderItems = request.items.map(it => {
    const min = seedMap.get(it.miniatureId);
    const name = min?.name || 'Sanctified Relic';
    const faction = min?.faction || 'Imperium';
    const price = min?.price || 40.00;
    const isStarter = min?.tags.includes('StarterSet') || false;
    return {
      miniatureId: it.miniatureId,
      miniatureName: name,
      faction,
      isStarterSet: isStarter,
      quantity: it.quantity,
      unitPrice: price
    };
  });

  const subtotal = orderItems.reduce((acc, curr) => acc + curr.unitPrice * curr.quantity, 0);
  const bundleDiscount = subtotal * 0.15;
  const promoDiscount = request.promoCode ? subtotal * 0.10 : 0;
  const total = Math.max(0, subtotal - bundleDiscount - promoDiscount);

  const localOrder: Order = {
    id: `ord-${Date.now()}`,
    orderNumber: `WH-${Math.floor(10000 + Math.random() * 90000)}`,
    customer: request.customer,
    items: orderItems,
    subtotal: Number(subtotal.toFixed(2)),
    bundleDiscountAmount: Number(bundleDiscount.toFixed(2)),
    promoDiscountAmount: Number(promoDiscount.toFixed(2)),
    appliedPromoCode: request.promoCode || null,
    totalAmount: Number(total.toFixed(2)),
    status: 'Processing',
    createdAt: new Date().toISOString()
  };

  const existing = getLocalOrders();
  saveLocalOrders([localOrder, ...existing]);
  return localOrder;
}

export async function fetchUserOrders(email: string): Promise<Order[]> {
  try {
    const res = await fetch(`/api/orders/user/${encodeURIComponent(email)}`);
    if (res.ok) {
      const data = await res.json();
      if (Array.isArray(data) && data.length > 0) {
        return data;
      }
    }
  } catch {
    // fallback
  }
  return getLocalOrders();
}

export async function fetchInventory(): Promise<InventoryItem[]> {
  try {
    const res = await fetch('/api/inventory');
    if (res.ok) {
      return await res.json();
    }
  } catch {
    // fallback
  }
  return getLocalInventory();
}

export async function adjustInventory(req: AdjustInventoryRequest): Promise<InventoryItem> {
  try {
    const res = await fetch('/api/inventory/adjust', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(req)
    });
    if (res.ok) {
      return await res.json();
    }
  } catch {
    // fallback
  }

  const items = getLocalInventory();
  const target = items.find(i => i.id === req.miniatureId);
  if (target) {
    target.availableStock = Math.max(0, target.availableStock + req.deltaQuantity);
    target.lastUpdatedAt = new Date().toISOString();
    saveLocalInventory(items);
    return target;
  }

  const newItem: InventoryItem = {
    id: req.miniatureId,
    miniatureName: 'Miniature Unit',
    availableStock: Math.max(0, req.deltaQuantity),
    reservedStock: 0,
    lastUpdatedAt: new Date().toISOString()
  };
  items.push(newItem);
  saveLocalInventory(items);
  return newItem;
}
