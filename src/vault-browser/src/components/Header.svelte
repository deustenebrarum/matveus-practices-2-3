<script lang="ts">
  import { cart } from '../lib/state/cart.svelte';
  import { ui } from '../lib/state/ui.svelte';
  import { user } from '../lib/state/user.svelte';

  let mobileSearchOpen = $state(false);
</script>

<!-- BEGIN: TopHeader matching design/index.html & design/account.html -->
<header class="sticky top-0 z-40 bg-[#0f1117]/95 border-b border-[#3b3221] backdrop-blur-md shadow-2xl">
  <div class="max-w-[1440px] mx-auto px-4 sm:px-6 h-20 flex items-center justify-between gap-4">
    <!-- Brand Crest & Name -->
    <a
      href="/"
      class="flex items-center gap-3 cursor-pointer group select-none"
      onclick={(e) => { e.preventDefault(); ui.navigateTo('catalog'); }}
    >
      <div class="relative w-12 h-10 flex items-center justify-center">
        <img
          src="https://images-wixmp-ed30a86b8c4ca887773594c2.wixmp.com/f/041252f3-b14e-4afe-9bf6-9e6db2cc38d6/d91enql-0f775463-ef24-4efc-9885-8dc574172e59.png/v1/fill/w_900,h_335/40k_imperial_aquila_transparent__by_fuguestock_d91enql-fullview.png?token=eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJzdWIiOiJ1cm46YXBwOjdlMGQxODg5ODIyNjQzNzNhNWYwZDQxNWVhMGQyNmUwIiwiaXNzIjoidXJuOmFwcDo3ZTBkMTg4OTgyMjY0MzczYTVmMGQ0MTVlYTBkMjZlMCIsIm9iaiI6W1t7ImhlaWdodCI6Ijw9MzM1IiwicGF0aCI6Ii9mLzA0MTI1MmYzLWIxNGUtNGFmZS05YmY2LTllNmRiMmNjMzhkNi9kOTFlbnFsLTBmNzc1NDYzLWVmMjQtNGVmYy05ODg1LThkYzU3NDE3MmU1OS5wbmciLCJ3aWR0aCI6Ijw9OTAwIn1dXSwiYXVkIjpbInVybjpzZXJ2aWNlOmltYWdlLm9wZXJhdGlvbnMiXX0.-vXS5nbicneuJrwcJ08aj7Ub59iyn-8qizni3cPv0mQ"
          alt="Imperial Aquila"
          class="w-full h-full object-contain filter drop-shadow-[0_0_8px_rgba(223,185,108,0.6)] group-hover:scale-105 transition-transform duration-300"
        />
      </div>
      <div class="flex flex-col">
        <span class="font-serif text-xl md:text-2xl font-black tracking-widest text-vault-brightGold drop-shadow uppercase font-cinzel">
          WARHAMMER VAULT
        </span>
        <span class="text-[9px] tracking-[0.3em] uppercase text-vault-gold/70 -mt-1 font-cinzel" style="margin-left: 1rem;">
          The Emperor's Armoury
        </span>
      </div>
    </a>

    <!-- Desktop Search Input Container -->
    <div class="flex-1 max-w-xl mx-4 hidden md:block">
      <div class="relative flex items-center w-full bg-transparent">
        <svg class="w-4 h-4 text-vault-brightGold shrink-0 absolute left-3 pointer-events-none" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" stroke-linecap="round" stroke-linejoin="round" stroke-width="2"></path>
        </svg>
        <input
          class="w-full rounded pl-9 pr-3 py-2 text-xs sm:text-sm text-gray-200 placeholder-gray-500 bg-[#0c0e14] border border-[#5a482b] focus:border-vault-gold focus:outline-none transition-colors"
          id="catalogSearch"
          placeholder="Search miniatures, factions, wargear, warscrolls..."
          type="text"
          value={ui.searchQuery}
          oninput={(e) => ui.setSearch(e.currentTarget.value)}
        />
      </div>
    </div>

    <!-- Right Actions: Navigation Links & Cart Trigger -->
    <div class="flex items-center gap-2 sm:gap-4">
      <!-- Mobile Search Toggle Button -->
      <button
        type="button"
        class="md:hidden p-2 text-vault-gold hover:text-white transition-colors cursor-pointer"
        aria-label="Toggle Search"
        onclick={() => { mobileSearchOpen = !mobileSearchOpen; }}
      >
        <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" stroke-linecap="round" stroke-linejoin="round" stroke-width="2"></path>
        </svg>
      </button>

      <!-- Catalog Link -->
      <a
        href="/"
        class="flex items-center gap-1.5 text-xs font-cinzel font-medium transition-colors px-2 py-1 cursor-pointer {ui.activeView === 'catalog' ? 'text-vault-brightGold border-b-2 border-vault-gold pb-0.5' : 'text-gray-300 hover:text-vault-brightGold'}"
        onclick={(e) => { e.preventDefault(); ui.navigateTo('catalog'); }}
      >
        Catalog
      </a>

      <!-- Account Link -->
      <a
        href="/account"
        class="flex items-center gap-2 text-xs font-cinzel font-medium transition-colors px-2 py-1 cursor-pointer {ui.activeView === 'account' ? 'text-vault-brightGold border-b-2 border-vault-gold pb-0.5' : 'text-gray-300 hover:text-vault-brightGold'}"
        onclick={(e) => { e.preventDefault(); ui.navigateTo('account'); }}
      >
        <svg class="w-4 h-4 text-vault-gold" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" stroke-linecap="round" stroke-linejoin="round" stroke-width="2"></path>
        </svg>
        <span class="hidden sm:inline">Account</span>
      </a>

      <!-- Slide-out Cart Button Trigger -->
      <button
        type="button"
        aria-label="Open Cart"
        class="relative flex items-center gap-2.5 px-3 sm:px-4 py-2 bg-[#141720] border border-[#7c6439] hover:border-vault-brightGold text-vault-brightGold transition-all shadow-lg hover:shadow-[0_0_15px_rgba(223,185,108,0.3)] group rounded-none cursor-pointer {ui.cartHeaderPulsing ? 'scale-105 border-yellow-400' : ''}"
        id="openCartBtn"
        onclick={() => ui.openCart()}
      >
        <div class="relative flex items-center">
          <svg class="w-5 h-5 text-vault-brightGold group-hover:scale-110 transition-transform" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path d="M3 3h2l.4 2M7 13h10l4-8H5.4M7 13L5.4 5M7 13l-2.293 2.293c-.63.63-.184 1.707.707 1.707H17m0 0a2 2 0 100 4 2 2 0 000-4zm-8 2a2 2 0 11-4 0 2 2 0 014 0z" stroke-linecap="round" stroke-linejoin="round" stroke-width="2"></path>
          </svg>
          {#if cart.count > 0}
            <span
              class="absolute -top-2.5 -right-2.5 bg-[#8a1c14] text-[#ffdad5] text-[10px] font-bold w-4 h-4 rounded-full flex items-center justify-center border border-[#ffb4a9]/60 shadow-[0_0_6px_rgba(255,100,100,0.6)] animate-pulse"
              id="cartBadgeCount"
            >
              {cart.count}
            </span>
          {/if}
        </div>
        <span class="font-cinzel text-xs tracking-wider text-gray-200 group-hover:text-white font-semibold hidden sm:inline">
          CART: <span class="text-vault-brightGold font-bold" id="cartHeaderAmount">${cart.total.toFixed(2)}</span>
        </span>
      </button>
    </div>
  </div>

  <!-- Mobile Collapsible Search Bar -->
  {#if mobileSearchOpen}
    <div class="md:hidden px-4 pb-3 pt-1 border-t border-[#3b3221] bg-[#0c0e14]">
      <div class="relative flex items-center w-full">
        <svg class="w-4 h-4 text-vault-brightGold shrink-0 absolute left-3 pointer-events-none" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" stroke-linecap="round" stroke-linejoin="round" stroke-width="2"></path>
        </svg>
        <input
          class="w-full rounded pl-9 pr-3 py-2 text-xs text-gray-200 placeholder-gray-500 bg-[#12141c] border border-[#5a482b] focus:border-vault-gold focus:outline-none"
          placeholder="Search miniatures, factions, wargear..."
          type="text"
          value={ui.searchQuery}
          oninput={(e) => ui.setSearch(e.currentTarget.value)}
        />
      </div>
    </div>
  {/if}
</header>
<!-- END: TopHeader -->
