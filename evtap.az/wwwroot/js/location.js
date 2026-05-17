// Cascading location dropdowns for the Home search form

async function loadDistricts(cityId, selectedId = null) {
    const distGroup = document.getElementById('districtGroup');
    const metroGroup = document.getElementById('metroGroup');
    const distSel = document.getElementById('districtSelect');
    const settlGroup = document.getElementById('settlementGroup');

    if (settlGroup) settlGroup.style.display = 'none';

    if (!cityId) {
        if (distGroup) distGroup.style.display = 'none';
        if (metroGroup) metroGroup.style.display = 'none';
        return;
    }

    try {
        const res = await fetch('/Listings/GetDistricts?cityId=' + cityId);
        const data = await res.json();

        if (distSel) {
            distSel.innerHTML = '<option value="">— Rayon seçin —</option>';
            data.forEach(d => {
                const opt = new Option(d.name, d.id, false, d.id == selectedId);
                distSel.add(opt);
            });
        }

        if (distGroup) distGroup.style.display = data.length ? '' : 'none';

        // Show metro only for Baku (id=1)
        if (metroGroup) {
            metroGroup.style.display = (cityId == 1) ? '' : 'none';
            if (cityId == 1) await loadMetroStations();
        }
    } catch (e) { console.error(e); }
}

async function loadSettlements(districtId, selectedId = null) {
    const settlGroup = document.getElementById('settlementGroup');
    const settlSel = document.getElementById('settlementSelect');

    if (!districtId) {
        if (settlGroup) settlGroup.style.display = 'none';
        return;
    }

    try {
        const res = await fetch('/Listings/GetSettlements?districtId=' + districtId);
        const data = await res.json();

        if (settlSel) {
            settlSel.innerHTML = '<option value="">— Qəsəbə —</option>';
            data.forEach(s => {
                const opt = new Option(s.name, s.id, false, s.id == selectedId);
                settlSel.add(opt);
            });
        }

        if (settlGroup) settlGroup.style.display = data.length ? '' : 'none';
    } catch (e) { console.error(e); }
}

async function loadMetroStations() {
    const metroSel = document.getElementById('metroSelect');
    if (!metroSel || metroSel.options.length > 1) return;
    try {
        const res = await fetch('/Listings/GetDistricts?cityId=0'); // placeholder - metro loaded from select list
    } catch (e) {}
}
