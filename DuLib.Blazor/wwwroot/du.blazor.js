function showDuLibBlazor(message) {
  return prompt(message, "DuLib Blazor!");
}

// cookie
DUCKIE = {
  rd: function () {
    return (document.cookie.length === 0) ? 'z=0' : decodeURIComponent(document.cookie);
  },
  wr: function (name, value, days) {
    document.cookie = `${name}=${value}${this.exr(days)};path=/`;
  },
  rm: function (name) {
    document.cookie = `${name}=;path=/;expires=${(new Date()).toGMTString()}`;
  },
  exr: function (days) {
    if (!Number.isFinite(days))
      return '';
    return `;expires=${this.afs(days).toGMTString()}`;
  },
  afs: function (days) {
    const date = new Date();
    date.setDate(date.getDate() + days);
    return date;
  }
}

// carousel
DUCARS = {
  init: function (e, orf, opt) {
    if (!e) return;
    const c = new bootstrap.Carousel(e, opt);
    e.orf = orf;
    e.addEventListener('slide.bs.carousel', this.bs);
    e.addEventListener('slid.bs.carousel', this.es);
    if (opt.ride === "carousel") c.cycle();
  },
  disp: function (e) {
    if (!e) return;
    e.removeEventListener('slide.bs.carousel', this.bs);
    e.removeEventListener('slid.bs.carousel', this.es);
    e.orf = null;
    this.g(e)?.dispose();
  },
  to: function (e, i) {
    this.g(e)?.to(i);
  },
  prev: function (e) {
    this.g(e)?.prev();
  },
  next: function (e) {
    this.g(e)?.next();
  },
  cycle: function (e) {
    this.g(e)?.cycle();
  },
  pause: function (e) {
    this.g(e)?.pause();
  },
  g: (e) => bootstrap.Carousel.getInstance(e),
  bs: (e) => e.target.orf.invokeMethodAsync('ivk_cars_bs', e.from, e.to, e.direction === 'left'),
  es: (e) => e.target.orf.invokeMethodAsync('ivk_cars_es', e.from, e.to, e.direction === 'left')
}

// dropdown
DUDROP = {
  init: function (e, orf, ref) {
    if (!e) return;
    e.orf = orf;
    e.addEventListener('shown.bs.dropdown', this.bs);
    e.addEventListener('hidden.bs.dropdown', this.bh);
    if (ref) {
      const r = document.querySelector(ref);
      const c = new bootstrap.Dropdown(e, { reference: r });
    }
    else { const c = new bootstrap.Dropdown(e); }
  },
  disp: function (e) {
    if (!e) return;
    e.addEventListener('shown.bs.dropdown', this.bs);
    e.addEventListener('hidden.bs.dropdown', this.bh);
    e.orf = null;
    this.g(e)?.dispose();
  },
  show: function (e) {
    this.g(e)?.show();
  },
  hide: function (e) {
    this.g(e)?.hide();
  },
  g: (e) => bootstrap.Dropdown.getInstance(e),
  bs: (e) => e.target.orf.invokeMethodAsync('ivk_drop_show'),
  bh: (e) => e.target.orf.invokeMethodAsync('ivk_drop_hide')
}
