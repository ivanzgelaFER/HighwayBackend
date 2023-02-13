$(document).on('click', '.deleterow', function () {
    event.preventDefault();
    var tr = $(this).parents("tr");
    tr.remove();
    clearOldMessage();
});

$(function () {
    $(".form-control").bind('keydown', function (event) {
        if (event.which === 13) {
            event.preventDefault();
        }
    });
    /*
     ovo je za enter onKeyPress() 

    $("#artikl-kolicina, #artikl-rabat").bind('keydown', function (event) {
        if (event.which === 13) {
            event.preventDefault();
            dodajArtikl();
        }
    });*/

    $("#odrzavanje-dodaj").click(function () {
        event.preventDefault();
        dodajOdrzavanje();
    });
});

function dodajOdrzavanje() {
    var sifra = $("#odrzavanje-sifra").val(); //+
    if (sifra != '') {
        if ($("[name='OdrzavanjeObjekata[" + sifra + "].Id'").length > 0) {
            alert('Odrzavanje vec postoji');
            return;
        }

        var naziv = $("#odrzavanje-imeFirme").val();                        //+
        var radnimDanomOd = $("#odrzavanjeObjekata-radnimDanomOd").val();   //+
        var radnimDanomDo = $("#odrzavanjeObjekata-radnimDanomDo").val();   //+
        var vikendimaOd = $("#odrzavanjeObjekata-vikendimaOd").val();       //+
        var vikendimaDo = $("#odrzavanjeObjekata-vikendimaDo").val();       //+
        var brojLjudi = $("#odrzavanjeObjekata-brojLjudi").val();           //+
        var cijena = $("#odrzavanjeObjekata-cijena").val();                 //+
        var predvidenoDana = $("#odrzavanjeObjekata-predvidenoDana").val(); //+
        var vrstaId = $("#odrzavanje-vrstaId").val();                       //+
        var vrsta = $("#odrzavanje-vrsta").val();                           //+
        var cestovniObjektId = $("#odrzavanje-cestovniObjekt").val();       //+

        //Alternativa ako su hr postavke sa zarezom //http://haacked.com/archive/2011/03/19/fixing-binding-to-decimals.aspx/
        //ili ovo http://intellitect.com/custom-model-binding-in-asp-net-core-1-0/

        var template = $('#template').html();
        template = template.replace(/--sifra--/g, sifra)
            .replace(/--naziv--/g, naziv)
            .replace(/--radnimDanomOd--/g, radnimDanomOd)
            .replace(/--radnimDanomDo--/g, radnimDanomDo)
            .replace(/--vikendimaOd--/g, vikendimaOd)
            .replace(/--vikendimaDo--/g, vikendimaDo)
            .replace(/--brojLjudi--/g, brojLjudi)
            .replace(/--cijena--/g, cijena)
            .replace(/--predvidenoDana--/g, predvidenoDana)
            .replace(/--vrstaId--/g, vrstaId)
            .replace(/--vrsta--/g, vrsta)
            .replace(/--cestovniObjekt--/g, cestovniObjektId)
        $(template).find('tr').insertBefore($("#table-odrzavanja").find('tr').last());

        $("#odrzavanje-imeFirme").val('');
        $("#odrzavanje-sifra").val('');
        $("#odrzavanjeObjekata-radnimDanomOd").val('');
        $("#odrzavanjeObjekata-radnimDanomDo").val('');
        $("#odrzavanjeObjekata-vikendimaOd").val('');
        $("#odrzavanjeObjekata-vikendimaDo").val('');
        $("#odrzavanjeObjekata-brojLjudi").val('');
        $("#odrzavanjeObjekata-cijena").val('');
        $("#odrzavanjeObjekata-predvidenoDana").val('');
        $("#odrzavanje-vrstaId").val('');
        $("#odrzavanje-vrsta").val('');

        clearOldMessage();
    }
}