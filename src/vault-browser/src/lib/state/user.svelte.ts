export interface SavedAddress {
  id: string;
  type: 'PVZ' | 'Courier' | 'Postal';
  city: string;
  line1: string;
  details: string;
  isPrimary: boolean;
}

export interface LoyaltyRank {
  name: string;
  percentage: number;
  threshold: number;
}

class UserState {
  name = $state<string>('Master Valtor (Vane Malcor)');
  email = $state<string>('inquisitor.malcor@imperium.vault');
  phone = $state<string>('+7 (999) 40K-1984');
  heraldry = $state<'Imperium' | 'Chaos' | 'Necrons' | 'Orks'>('Imperium');

  expendedRubles = $state<number>(34500);
  nextRankThreshold = $state<number>(50000);
  currentRankName = $state<string>('Veteran (7%)');
  discountRate = $state<number>(0.07);

  activeOrderNumber = $state<string>('WH-84920');
  activeOrderDate = $state<string>('Sep 15, 2026');
  activeCourier = $state<string>('CDEK Express');
  activeTrackingCipher = $state<string>('CDEK-40K-8492091');
  activeStep = $state<number>(3); // 1, 2, 3, 4
  activeDestination = $state<string>('Moscow, CDEK PVZ #104 (Tverskaya St, 12)');

  addresses = $state<SavedAddress[]>([
    {
      id: 'addr-1',
      type: 'PVZ',
      city: 'Moscow',
      line1: 'CDEK PVZ #104, Tverskaya St, 12, bld. 2',
      details: 'Operating Hours: 10:00 - 21:00',
      isPrimary: true
    },
    {
      id: 'addr-2',
      type: 'Courier',
      city: 'Saint Petersburg',
      line1: 'Courier Delivery, Nevsky Ave, 45, apt. 18',
      details: 'Intercom: 18K • Entrance 2',
      isPrimary: false
    },
    {
      id: 'addr-3',
      type: 'Postal',
      city: 'Yekaterinburg',
      line1: 'Postal Office #620000, Lenina St, 39',
      details: 'Postal Index: 620000',
      isPrimary: false
    }
  ]);

  setPrimaryAddress(id: string) {
    for (const a of this.addresses) {
      a.isPrimary = a.id === id;
    }
  }

  deleteAddress(id: string) {
    this.addresses = this.addresses.filter(a => a.id !== id);
  }

  addAddress(address: Omit<SavedAddress, 'id'>) {
    const id = `addr-${Date.now()}`;
    if (address.isPrimary) {
      for (const a of this.addresses) a.isPrimary = false;
    }
    this.addresses.push({ ...address, id });
  }

  setHeraldry(h: 'Imperium' | 'Chaos' | 'Necrons' | 'Orks') {
    this.heraldry = h;
  }
}

export const user = new UserState();
