import type {
  Miniature,
  FactionCount,
  CreateOrderRequest,
  Order,
  InventoryItem,
  AdjustInventoryRequest
} from '../types';

export interface CatalogQueryParams {
  q?: string;
  universe?: string;
  faction?: string;
  tag?: string;
  minPrice?: number;
  maxPrice?: number;
  sort?: 'featured' | 'price_asc' | 'price_desc' | 'name' | string;
  page?: number;
  pageSize?: number;
}

export interface CatalogResponse {
  total: number;
  page: number;
  pageSize: number;
  items: Miniature[];
}

function getApiUrl(path: string): string {
  // In Node.js / test environments or server context, use VITE_API_URL if available
  const isNode = typeof window === 'undefined';
  const configuredBase = (typeof import.meta !== 'undefined' && import.meta.env?.VITE_API_URL)
    ? import.meta.env.VITE_API_URL.replace(/\/$/, '')
    : '';

  if (isNode && configuredBase) {
    return `${configuredBase}${path.startsWith('/') ? path : '/' + path}`;
  }
  // In browser, relative URL delegates to Vite dev proxy or Nginx reverse proxy
  return path.startsWith('/') ? path : `/${path}`;
}

async function handleResponse<T>(res: Response): Promise<T> {
  if (!res.ok) {
    let errorDetail = `Request failed with status ${res.status} ${res.statusText}`;
    try {
      const errJson = await res.json();
      if (errJson.title || errJson.message) {
        errorDetail = errJson.title || errJson.message;
      } else if (errJson.errors) {
        errorDetail = Object.values(errJson.errors).flat().join(' ');
      }
    } catch {
      // ignore non-json error responses
    }
    throw new Error(errorDetail);
  }
  return res.json() as Promise<T>;
}

export async function fetchMiniatures(params?: CatalogQueryParams): Promise<CatalogResponse> {
  const query = new URLSearchParams();
  if (params?.q?.trim()) query.set('q', params.q.trim());
  if (params?.universe?.trim()) query.set('universe', params.universe.trim());
  if (params?.faction?.trim()) query.set('faction', params.faction.trim());
  if (params?.tag?.trim()) query.set('tag', params.tag.trim());
  if (params?.minPrice !== undefined) query.set('minPrice', params.minPrice.toString());
  if (params?.maxPrice !== undefined) query.set('maxPrice', params.maxPrice.toString());
  if (params?.sort) query.set('sort', params.sort);
  if (params?.page) query.set('page', params.page.toString());
  if (params?.pageSize) query.set('pageSize', params.pageSize.toString());

  const queryString = query.toString();
  const url = getApiUrl(`/api/catalog${queryString ? `?${queryString}` : ''}`);

  const res = await fetch(url, { method: 'GET', credentials: 'omit' });
  const data = await handleResponse<any>(res);

  if (Array.isArray(data)) {
    return {
      total: data.length,
      page: 1,
      pageSize: data.length,
      items: data
    };
  }

  return {
    total: data.total ?? data.items?.length ?? 0,
    page: data.page ?? 1,
    pageSize: data.pageSize ?? 20,
    items: data.items ?? []
  };
}

export async function fetchMiniatureById(id: string): Promise<Miniature> {
  const res = await fetch(getApiUrl(`/api/catalog/${encodeURIComponent(id)}`));
  return handleResponse<Miniature>(res);
}

export async function fetchFactions(): Promise<FactionCount[]> {
  const res = await fetch(getApiUrl('/api/catalog/factions'));
  return handleResponse<FactionCount[]>(res);
}

export async function createOrder(request: CreateOrderRequest): Promise<Order> {
  const res = await fetch(getApiUrl('/api/orders'), {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(request)
  });
  return handleResponse<Order>(res);
}

export async function fetchOrderById(id: string): Promise<Order> {
  const res = await fetch(getApiUrl(`/api/orders/${encodeURIComponent(id)}`));
  return handleResponse<Order>(res);
}

export async function fetchUserOrders(email: string): Promise<Order[]> {
  const res = await fetch(getApiUrl(`/api/orders/user/${encodeURIComponent(email)}`));
  return handleResponse<Order[]>(res);
}

export async function fetchInventory(): Promise<InventoryItem[]> {
  const res = await fetch(getApiUrl('/api/inventory'));
  return handleResponse<InventoryItem[]>(res);
}

export async function adjustInventory(req: AdjustInventoryRequest): Promise<InventoryItem> {
  const res = await fetch(getApiUrl('/api/inventory/adjust'), {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(req)
  });
  return handleResponse<InventoryItem>(res);
}
