// helper function from http://www.jquerybyexample.net/2012/06/get-url-parameters-using-jquery.html
function GetURLParameter(sParam) {
    var sPageURL = window.location.search.substring(1);
    var sURLVariables = sPageURL.split('&');
    for (var i = 0; i < sURLVariables.length; i++) {
        var sParameterName = sURLVariables[i].split('=');
        if (sParameterName[0] == sParam) {
            return sParameterName[1];
        }
    }
}

function update_td_style(td, mean, mean_difference) {
    if (mean <= 2) {
        td.css("background-color", "#00ff00");
    }
    if (mean >= 3) {
        td.css("background-color", "#ff0000");
        td.css("color", "#ffffff");
    }
    if (mean_difference <= -1) {
        td.css("font-weight", "bold");
    }
}

function create_td(tr, data, to_style, to_center) {
    var td = $(document.createElement("td"));
    if (to_style === true) {
        update_td_style(td, data.mean, data.mean_difference);
    }
    if (to_center === true) {
        td.attr("class", "text-center");
    }
    tr.append(td);
    return td;
}

function create_link_ref(tr, data) {
    var td = create_td(tr, data, false, false);
    var a_ref = $(document.createElement("a"));
    a_ref.attr("href", "http://www.reuters.com/finance/stocks/analyst?symbol=" + data.code);
    a_ref.attr("target", "_blank");
    a_ref.html(data.code);
    td.append(a_ref);
}

function create_remove_button(tr, data) {
    var td = create_td(tr, data, false, false);
    var form = $(document.createElement("form"));
    var input = $(document.createElement("input"));
    var button = $(document.createElement("button"));
    var span = $(document.createElement("span"));

    form.attr("action", "RemoveStock");

    input.attr("type", "hidden");
    input.attr("name", "stock");
    input.attr("value", data.code);

    button.attr("type", "submit");
    button.attr("class", "btn btn-default");

    span.attr("class", "glyphicon glyphicon-minus text-danger");
    button.html("&nbsp;");

    button.append(span);
    form.append(input);
    form.append(button);
    td.append(form);
}

function add_category_elements(tr, data) {
    var category_td = create_td(tr, data, false, false);
    var category_remove_td = create_td(tr, data, false, false);
    if (!data.hasOwnProperty("categories")) {
        return;
    }
    var categories = data.categories;
    var length = categories.length;
    var i = 0;
    var form, input, button, p, span;

    for (i = 0; i < length; i++) {
        p = $(document.createElement("p"));
        p.html(categories[i]);
        category_td.append(p);

        form = $(document.createElement("form"));
        button = $(document.createElement("button"));
        span = $(document.createElement("span"));

        form.attr("action", "RemoveStockCategory");

        input = $(document.createElement("input"));
        input.attr("type", "hidden");
        input.attr("name", "stock");
        input.attr("value", data.code);
        form.append(input);

        input = $(document.createElement("input"));
        input.attr("type", "hidden");
        input.attr("name", "category");
        input.attr("value", categories[i]);
        form.append(input);

        button.attr("type", "submit");
        button.attr("class", "btn btn-default");

        span.attr("class", "glyphicon glyphicon-minus text-danger");
        button.html("&nbsp;");
        button.append(span);
        form.append(button);
        category_remove_td.append(form);
    }
}

function create_tr(data) {
    var tbody = $("tbody.stock-tbody");
    var tr = $(document.createElement("tr"));

    // create all the data elements
    create_td(tr, data, true, false).html(data.code);
    create_td(tr, data, true, false).html(data.description);
    create_td(tr, data, true, true).html(data.buy);
    create_td(tr, data, true, true).html(data.outperform);
    create_td(tr, data, true, true).html(data.hold);
    create_td(tr, data, true, true).html(data.underperform);
    create_td(tr, data, true, true).html(data.sell);
    create_td(tr, data, true, true).html(data.mean);
    create_td(tr, data, true, true).html(data.mean_difference);
    create_td(tr, data, true, false).html(data.consensus);
    create_td(tr, data, true, true).html(data.dividend);
    create_td(tr, data, true, true).html(data.price_earnings);
    create_link_ref(tr, data);
    create_remove_button(tr, data);
    add_category_elements(tr, data);

    tbody.append(tr);

    var progress = $("progress.stock-progress");
    var progress_value = parseInt(progress.attr("value"));
    progress.attr("value", progress_value + 1);
}

function parse_stock_list(data) {
    var stocks = [];
    var i = 0;
    var length = data.length;

    for (i = 0; i < length; i++) {
        stocks.push(data[i]);
    }

    length = stocks.length;

    var progress = $("progress.stock-progress");
    progress.attr("max", length);
    progress.attr("value", 0);
    progress.removeAttr("hidden");

    for (i = 0; i < length; i++) {
        $.ajax({
            url: "/Stocks/Info?stock=" + stocks[i],
        }).done(create_tr);
    }

    var select = $("select.associate-select-stock");
    populate_select(select, stocks);

    var form = $("form.associate-form");
    form.removeAttr("hidden");
}

function parse_category_list(data) {
    var categories = [];
    var i = 0;
    var length = data.length;

    for (i = 0; i < length; i++) {
        categories.push(data[i]);
    }

    // TODO: fix this mess!
    var select = $("select.associate-select-category");
    populate_select(select, categories);

    select = $("select.remove-category-select");
    populate_select(select, categories);

    var form = $("form.remove-category-form");
    form.removeAttr("hidden");

    select = $("select.filter-category-select");
    populate_select(select, categories);

    form = $("form.filter-form");
    form.removeAttr("hidden");

    select = $("select.disassociate-category-select");
    populate_select(select, categories);

    form = $("form.disassociate-category-form");
    form.removeAttr("hidden");
}

function populate_select(select, data) {
    var length = data.length;
    var i = 0;
    var option;

    // clear the select's html
    select.html("");

    for (i = 0; i < length; i++) {
        option = $(document.createElement("option"));
        option.attr("value", data[i]);
        option.html(data[i]);
        select.append(option);
    }
}

$(document).ready(function () {
    var category = GetURLParameter("category");
    var category_filter = "";
    if (typeof (category) !== 'undefined') {
        category_filter = "?category=" + category;
    }
    $.ajax({
        url: "/stocks/StockList" + category_filter,
    }).done(parse_stock_list);

    $.ajax({
        url: "/stocks/CategoryList",
    }).done(parse_category_list);
});