export interface SavedAddress {
  id: string;
  type: 'PVZ' | 'Courier' | 'Postal';
  city: string;
  line1: string;
  details: string;
  isPrimary: boolean;
}

const STORAGE_SESSION_KEY = 'vault_user_session_v2';
const STORAGE_ADDRESSES_KEY = 'vault_user_addresses_v2';

// Clean up old legacy keys that may have held mock profiles like "Master Valtor"
if (typeof localStorage !== 'undefined') {
  try {
    localStorage.removeItem('vault_user_profile');
    localStorage.removeItem('vault_user_addresses');
    localStorage.removeItem('vault_user_orders');
    localStorage.removeItem('vault_user_inventory');
  } catch {
    // ignore
  }
}

interface StoredUserProfile {
  name: string;
  email: string;
  phone: string;
  heraldry: 'Imperium' | 'Chaos' | 'Necrons' | 'Orks';
}

function loadStoredProfile(): StoredUserProfile | null {
  if (typeof localStorage === 'undefined') return null;
  try {
    const raw = localStorage.getItem(STORAGE_SESSION_KEY);
    if (raw) return JSON.parse(raw);
  } catch {
    // ignore
  }
  return null;
}

function loadStoredAddresses(): SavedAddress[] {
  if (typeof localStorage === 'undefined') return [];
  try {
    const raw = localStorage.getItem(STORAGE_ADDRESSES_KEY);
    if (raw) return JSON.parse(raw);
  } catch {
    // ignore
  }
  return [];
}

class UserState {
  private initialProfile = loadStoredProfile();

  isLoggedIn = $state<boolean>(!!this.initialProfile?.email);
  name = $state<string>(this.initialProfile?.name || '');
  email = $state<string>(this.initialProfile?.email || '');
  phone = $state<string>(this.initialProfile?.phone || '');
  heraldry = $state<'Imperium' | 'Chaos' | 'Necrons' | 'Orks'>(this.initialProfile?.heraldry || 'Imperium');

  addresses = $state<SavedAddress[]>(loadStoredAddresses());

  isStaff = $derived(
    this.isLoggedIn && (
      this.email.endsWith('@imperium.vault') ||
      this.email.endsWith('@munitorum.admin') ||
      this.email === 'admin@vault.local'
    )
  );

  primaryAddress = $derived(
    this.addresses.find(a => a.isPrimary) || this.addresses[0] || null
  );

  private persist() {
    if (typeof localStorage === 'undefined') return;
    try {
      if (this.isLoggedIn && this.email) {
        localStorage.setItem(STORAGE_SESSION_KEY, JSON.stringify({
          name: this.name,
          email: this.email,
          phone: this.phone,
          heraldry: this.heraldry
        }));
      } else {
        localStorage.removeItem(STORAGE_SESSION_KEY);
      }
      localStorage.setItem(STORAGE_ADDRESSES_KEY, JSON.stringify(this.addresses));
    } catch {
      // ignore
    }
  }

  login(email: string, name?: string, phone?: string) {
    this.email = email.trim().toLowerCase();
    this.name = name?.trim() || this.email.split('@')[0];
    if (phone) this.phone = phone.trim();
    this.isLoggedIn = true;
    this.persist();
  }

  logout() {
    this.isLoggedIn = false;
    this.email = '';
    this.name = '';
    this.phone = '';
    this.persist();
  }

  setHeraldry(h: 'Imperium' | 'Chaos' | 'Necrons' | 'Orks') {
    this.heraldry = h;
    this.persist();
  }

  updateProfile(name: string, phone: string) {
    this.name = name.trim();
    this.phone = phone.trim();
    this.persist();
  }

  setPrimaryAddress(id: string) {
    for (const a of this.addresses) {
      a.isPrimary = a.id === id;
    }
    this.persist();
  }

  deleteAddress(id: string) {
    this.addresses = this.addresses.filter(a => a.id !== id);
    this.persist();
  }

  addAddress(address: Omit<SavedAddress, 'id'>) {
    const id = `addr-${Date.now()}`;
    if (address.isPrimary) {
      for (const a of this.addresses) a.isPrimary = false;
    }
    this.addresses.push({ ...address, id });
    this.persist();
  }
}

export const user = new UserState();
