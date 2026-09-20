import test from 'node:test';
import assert from 'node:assert';
import {
  fetchMiniatures,
  fetchFactions,
  createOrder,
  fetchUserOrders,
  fetchInventory,
  adjustInventory
} from './api.ts';

// Unit tests for API client functions verifying contracts and URL/request formatting
test('fetchMiniatures formats query parameters and parses paginated catalog response', async () => {
  const originalFetch = globalThis.fetch;
  let requestedUrl = '';

  globalThis.fetch = (async (input: RequestInfo | URL) => {
    requestedUrl = input.toString();
    return new Response(JSON.stringify({
      total: 1,
      page: 1,
      pageSize: 20,
      items: [
        {
          id: '11111111-1111-1111-1111-111111111111',
          name: 'Roboute Guilliman, Primarch of the Ultramarines',
          price: 65.00,
          faction: 'Imperium',
          universe: 'Warhammer 40,000',
          imageUrl: 'https://lh3.googleusercontent.com/...',
          tags: ['Hero', 'Imperium'],
          isFeatured: true
        }
      ]
    }), { status: 200, headers: { 'Content-Type': 'application/json' } });
  }) as typeof globalThis.fetch;

  try {
    const res = await fetchMiniatures({
      q: 'Guilliman',
      faction: 'Imperium',
      minPrice: 50,
      maxPrice: 100,
      sort: 'price_asc'
    });

    assert.ok(requestedUrl.includes('/api/catalog?'));
    assert.ok(requestedUrl.includes('q=Guilliman'));
    assert.ok(requestedUrl.includes('faction=Imperium'));
    assert.ok(requestedUrl.includes('minPrice=50'));
    assert.ok(requestedUrl.includes('maxPrice=100'));
    assert.ok(requestedUrl.includes('sort=price_asc'));

    assert.strictEqual(res.total, 1);
    assert.strictEqual(res.items.length, 1);
    assert.strictEqual(res.items[0].price, 65.00);
    assert.strictEqual(res.items[0].name, 'Roboute Guilliman, Primarch of the Ultramarines');
  } finally {
    globalThis.fetch = originalFetch;
  }
});

test('fetchFactions requests /api/catalog/factions and parses summaries', async () => {
  const originalFetch = globalThis.fetch;

  globalThis.fetch = (async () => {
    return new Response(JSON.stringify([
      { faction: 'Imperium', count: 4, universe: 'Warhammer 40,000' },
      { faction: 'Chaos', count: 3, universe: 'Warhammer 40,000' }
    ]), { status: 200, headers: { 'Content-Type': 'application/json' } });
  }) as typeof globalThis.fetch;

  try {
    const factions = await fetchFactions();
    assert.strictEqual(factions.length, 2);
    assert.strictEqual(factions[0].faction, 'Imperium');
    assert.strictEqual(factions[0].count, 4);
  } finally {
    globalThis.fetch = originalFetch;
  }
});

test('createOrder posts command to /api/orders and returns created order', async () => {
  const originalFetch = globalThis.fetch;
  let postedBody = '';

  globalThis.fetch = (async (input: RequestInfo | URL, init?: RequestInit) => {
    postedBody = init?.body?.toString() || '';
    return new Response(JSON.stringify({
      id: 'ord-12345',
      orderNumber: 'ORD-20260920-1001',
      customer: {
        fullName: 'Captain Titus',
        email: 'titus@ultramarines.chapter.org',
        phone: '+79991234567',
        shippingAddress: 'Battle Barge Spear of Macragge',
        city: 'Graia Orbit',
        courierService: 'Orbital Drop'
      },
      items: [
        {
          miniatureId: '22222222-2222-2222-2222-222222222222',
          miniatureName: 'Space Marines: Intercessor Squad',
          faction: 'Imperium',
          isStarterSet: false,
          quantity: 2,
          unitPrice: 60.00
        }
      ],
      subtotal: 120.00,
      bundleDiscountAmount: 0,
      promoDiscountAmount: 12.00,
      appliedPromoCode: 'WARHAMMER10',
      totalAmount: 108.00,
      status: 'Confirmed',
      createdAt: '2026-09-20T12:00:00Z'
    }), { status: 201, headers: { 'Content-Type': 'application/json' } });
  }) as typeof globalThis.fetch;

  try {
    const order = await createOrder({
      customer: {
        fullName: 'Captain Titus',
        email: 'titus@ultramarines.chapter.org',
        phone: '+79991234567',
        shippingAddress: 'Battle Barge Spear of Macragge',
        city: 'Graia Orbit',
        courierService: 'Orbital Drop'
      },
      items: [
        { miniatureId: '22222222-2222-2222-2222-222222222222', quantity: 2 }
      ],
      promoCode: 'WARHAMMER10'
    });

    assert.ok(postedBody.includes('titus@ultramarines.chapter.org'));
    assert.strictEqual(order.orderNumber, 'ORD-20260920-1001');
    assert.strictEqual(order.totalAmount, 108.00);
    assert.strictEqual(order.promoDiscountAmount, 12.00);
  } finally {
    globalThis.fetch = originalFetch;
  }
});

test('fetchInventory and adjustInventory communicate with /api/inventory', async () => {
  const originalFetch = globalThis.fetch;

  globalThis.fetch = (async (input: RequestInfo | URL, init?: RequestInit) => {
    const url = input.toString();
    if (url.includes('/adjust')) {
      const parsed = JSON.parse(init?.body?.toString() || '{}');
      return new Response(JSON.stringify({
        id: parsed.miniatureId,
        miniatureName: 'Intercessors',
        availableStock: 30,
        reservedStock: 0,
        lastUpdatedAt: new Date().toISOString()
      }), { status: 200, headers: { 'Content-Type': 'application/json' } });
    }
    return new Response(JSON.stringify([
      {
        id: '22222222-2222-2222-2222-222222222222',
        miniatureName: 'Intercessors',
        availableStock: 25,
        reservedStock: 0,
        lastUpdatedAt: new Date().toISOString()
      }
    ]), { status: 200, headers: { 'Content-Type': 'application/json' } });
  }) as typeof globalThis.fetch;

  try {
    const items = await fetchInventory();
    assert.strictEqual(items.length, 1);
    assert.strictEqual(items[0].availableStock, 25);

    const updated = await adjustInventory({
      miniatureId: '22222222-2222-2222-2222-222222222222',
      deltaQuantity: 5
    });
    assert.strictEqual(updated.availableStock, 30);
  } finally {
    globalThis.fetch = originalFetch;
  }
});
