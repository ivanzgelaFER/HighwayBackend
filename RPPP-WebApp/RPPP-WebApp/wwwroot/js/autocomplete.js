$(function () {

  $("[data-autocomplete]").each(function (index, element) {
    var action = $(element).data('autocomplete');
    var resultplaceholder = $(element).data('autocomplete-placeholder-name');
    if (resultplaceholder === undefined)
      resultplaceholder = action;

    $(element).change(function () {
      var dest = $(`[data-autocomplete-placeholder='${resultplaceholder}']`);
      var text = $(element).val();
      if (text.length === 0 || text !== $(dest).data('selected-label')) {
        $(dest).val('');
      }
    });

    $(element).autocomplete({
      source: window.applicationBaseUrl + "autocomplete/" + action,
      autoFocus: true,
      minLength: 1,
      select: function (event, ui) {
        $(element).val(ui.item.label);
        var dest = $(`[data-autocomplete-placeholder='${resultplaceholder}']`);
        $(dest).val(ui.item.id);
        $(dest).data('selected-label', ui.item.label);


        var dest_vrstaId = $(`[data-autocomplete-placeholder-vrstaId='${resultplaceholder}']`);
        if (dest_vrstaId !== undefined) {
            $(dest_vrstaId).val(ui.item.vrstaId);
          }

        var dest_vrstaNaziv = $(`[data-autocomplete-placeholder-vrsta='${resultplaceholder}']`);
        if (dest_vrstaNaziv !== undefined) {
            $(dest_vrstaNaziv).val(ui.item.vrsta);
        }

        var dest_cestovniObjektId = $(`[data-autocomplete-placeholder-cestovniObjektId='${resultplaceholder}']`);
        if (dest_cestovniObjektId !== undefined) {
            $(dest_cestovniObjektId).val(ui.item.cestovniObjektId);
        }
         
        var dest_cijena = $(`[data-autocomplete-placeholder-cijena='${resultplaceholder}']`);
        if (dest_cijena !== undefined) {
          $(dest_cijena).val(ui.item.cijena);
        }

        var dest_rdOd = $(`[data-autocomplete-placeholder-radnimDanomOd='${resultplaceholder}']`);
          if (dest_rdOd !== undefined) {
              $(dest_rdOd).val(ui.item.radnimDanomOd);
        }

        var dest_rdDo = $(`[data-autocomplete-placeholder-radnimDanomDo='${resultplaceholder}']`);
          if (dest_rdDo !== undefined) {
              $(dest_rdDo).val(ui.item.radnimDanomDo);
        }

        var dest_vOd = $(`[data-autocomplete-placeholder-vikendimaOd='${resultplaceholder}']`);
          if (dest_vOd !== undefined) {
              $(dest_vOd).val(ui.item.vikendimaOd);
        }

        var dest_vDo = $(`[data-autocomplete-placeholder-vikendimaDo='${resultplaceholder}']`);
          if (dest_vDo !== undefined) {
              $(dest_vDo).val(ui.item.vikendimaDo);
        }

        var dest_brLjudi = $(`[data-autocomplete-placeholder-brojLjudi='${resultplaceholder}']`);
        if (dest_brLjudi !== undefined) {
            $(dest_brLjudi).val(ui.item.brojLjudi);
        }

        var dest_pDana = $(`[data-autocomplete-placeholder-predvidenoDana='${resultplaceholder}']`);
        if (dest_pDana !== undefined) {
            $(dest_pDana).val(ui.item.predvidenoDana);
        }
      }
    });
  });
});