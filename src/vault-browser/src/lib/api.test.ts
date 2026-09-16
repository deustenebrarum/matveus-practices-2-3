import test from 'node:test';
import assert from 'node:assert';
import {
  SEED_MINIATURES,
  fetchMiniatures,
  fetchFactions,
  createOrder,
  adjustInventory,
  fetchInventory
} from './api.ts';

test('SEED_MINIATURES has all required prototypes including Ork Nob Veteran', () => {
  assert.strictEqual(SEED_MINIATURES.length, 10);
  
  const captain = SEED_MINIATURES.find(m => m.name === 'Space Marine Captain');
  assert.ok(captain, 'Space Marine Captain exists');
  assert.strictEqual(captain?.price, 42.00);

  const necron = SEED_MINIATURES.find(m => m.name === 'Necron Overlord');
  assert.ok(necron, 'Necron Overlord exists');
  assert.strictEqual(necron?.price, 48.00);

  const orkNob = SEED_MINIATURES.find(m => m.name === 'Ork Nob Veteran');
  assert.ok(orkNob, 'Ork Nob Veteran exists');
  assert.strictEqual(orkNob?.price, 38.00);
});

test('fetchFactions returns aggregated factions and counts', async () => {
  const factions = await fetchFactions();
  assert.ok(factions.length > 0);
  const imp = factions.find(f => f.faction === 'Imperium');
  assert.ok(imp);
  assert.ok(imp.count >= 2);
});

test('createOrder fallback calculates 10% bundle discount and 10% promo discount', async () => {
  const order = await createOrder({
    customer: {
      fullName: 'Inquisitor Malcor',
      email: 'malcor@vault.io',
      phone: '+79991234567',
      shippingAddress: 'Tverskaya 12',
      city: 'Moscow',
      courierService: 'CDEK Express'
    },
    items: [
      { miniatureId: '11111111-1111-1111-1111-111111111111', quantity: 1 }, // 42.00
      { miniatureId: '99999999-9999-9999-9999-999999999999', quantity: 1 }  // 60.00
    ],
    promoCode: 'TERRA10'
  });

  assert.strictEqual(order.subtotal, 102.00);
  assert.strictEqual(order.bundleDiscountAmount, 10.20); // 10% of 102.00
  assert.strictEqual(order.promoDiscountAmount, 9.18);   // 10% of (102.00 - 10.20) = 9.18
  assert.strictEqual(order.totalAmount, 82.62);         // 102 - 10.20 - 9.18 = 82.62
});

test('adjustInventory adjusts stock count without negative bounds', async () => {
  const initial = await fetchInventory();
  const targetId = initial[0].id;
  const initialStock = initial[0].availableStock;

  const increased = await adjustInventory({ miniatureId: targetId, deltaQuantity: 5 });
  assert.strictEqual(increased.availableStock, initialStock + 5);

  const reduced = await adjustInventory({ miniatureId: targetId, deltaQuantity: -100 });
  assert.strictEqual(reduced.availableStock, 0);
});
