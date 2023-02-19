// dropdown
const DUDROP = {
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
