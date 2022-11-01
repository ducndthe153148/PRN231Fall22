const API_URL_BASE = "http://localhost:5000/api/Order"

function Login() {
    $.ajax({
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        type: "POST",
        url: `${API_URL_BASE}/login`,
        data: payload,
        dataType: "json",
        success: function (result) {
            console.log(result);
            localStorage.setItem('token', result["tolen"]);
            console.log(localStorage.getItem("token"));
            window.location.href = 'index.html'
        },
        error: function (req, status, err) {
            alert(err);
        }
    });
}

function LoadOrder() {
    $.ajax({
        url: `${API_URL_BASE}/GetAllOrders`,
        headers: { Authorization: 'Bearer ' + localStorage.getItem("token") },
        type: 'GET',
        success: function (rs) {
            $("#orders tbody tr").remove();
            var str = ` <tr>
                                  <th>OrderID</th>
                                  <th>OrderDate</th>
                                  <th>RequiredDate</th>
                                  <th>ShippedDate</th>
                                  <th>Freight($)</th>
                                  <th>Status</th>
                                  <th></th>
                                </tr>`;
            $.each(rs, function (i, item) {
                str += "<tr>";
                str += "<td scope='row'>" + `<a href="order-detail.html?orderId=${item.orderId}">` + item.orderId + '</a>' + "</td>";
                str += "<td>" + item.orderDate + "</td>";
                str += "<td>" + item.requiredDate + "</td>";
                str += "<td>" + item.shippedDate + "</td>";
                str += "<td>" + item.freight + "</td>";
                str += 
                `<td>
                   Compl &nbsp;| <a href="#" onclick="CancelOrder(${item.orderId}); return false;">Cancel</a>
                </td>`;
                str += "</tr>"
            });
            $("#orders").append(str);
        }
    });
}

function LoadEmployee() {
    $.ajax({
        url: `${API_URL_BASE}/GetEmployees`,
        headers: { Authorization: 'Bearer ' + localStorage.getItem("token") },
        type: 'GET',
        success: function (rs) {
            $.each(rs, function (key, value) {
                $('ul').append($('<li>').text(value.lastName).attr('value', value.employeeId)).trigger("chosen:updated");
            });
        }
    });
}

$("ul").on("click", "li", function () {
    var id = $(this).val();
    console.log(id);
    FilterEmployee(id);
});


function FilterEmployee(id) {
    $.ajax({
        url: `${API_URL_BASE}/GetOrdersByEmpID/${id}`,
        headers: { Authorization: 'Bearer ' + localStorage.getItem("token") },
        type: 'GET',
        success: function (rs) {
            $("#orders tr").remove();
            var str = ` <tr>
                                  <th>OrderID</th>
                                  <th>OrderDate</th>
                                  <th>RequiredDate</th>
                                  <th>ShippedDate</th>
                                  <th>Freight($)</th>
                                  <th>Status</th>
                                  <th></th>
                                </tr>`;
            $.each(rs, function (i, item) {
                str += "<tr>";
                str += "<td scope='row'>" + `<a href="order-detail.html?orderId=${item.orderId}">` + item.orderId + '</a>' + "</td>";
                str += "<td>" + item.orderDate + "</td>";
                str += "<td>" + item.requiredDate + "</td>";
                str += "<td>" + item.shippedDate + "</td>";
                str += "<td>" + item.freight + "</td>";
                str += `<td>
                   Compl &nbsp;| <a href="#" onclick="CancelOrder(${item.orderId}); return false;">Cancel</a>
                </td>`;
                str += "</tr>"
            });
            $("#orders").append(str);
        }
    });
}


function LoadOrderDetail() {

}

$("#filter").click(function (e) {
    let payload = JSON.stringify({
        from: $("#from").val(),
        to: $("#to").val()
    });
    let from = document.getElementById("from").value;
    let to = document.getElementById("to").value;
    console.log(from);
    $.ajax({
        url: `${API_URL_BASE}/FilterDate/${from}/${to}`,
        headers: { Authorization: 'Bearer ' + localStorage.getItem("token") },
        type: 'GET',
        success: function (rs) {
            $("#orders tr").remove();
            var str = ` <tr>
                                  <th>OrderID</th>
                                  <th>OrderDate</th>
                                  <th>RequiredDate</th>
                                  <th>ShippedDate</th>
                                  <th>Freight($)</th>
                                  <th>Status</th>
                                  <th></th>
                                </tr>`;
            $.each(rs, function (i, item) {
                str += "<tr>";
                str += "<td scope='row'>" + `<a href="order-detail.html?orderId=${item.orderId}">` + item.orderId + '</a>' + "</td>";
                str += "<td>" + item.orderDate + "</td>";
                str += "<td>" + item.requiredDate + "</td>";
                str += "<td>" + item.shippedDate + "</td>";
                str += "<td>" + item.freight + "</td>";
                str += `<td>
                   Compl &nbsp;| <a href="#" onclick="CancelOrder(${item.orderId}); return false;">Cancel</a>
                </td>`;
                str += "</tr>"
            });
            $("#orders").append(str);
        },
        error: function (req, status, err) {
            alert(err);
        }
    });
})

function CancelOrder(id) {
    $.ajax({
        type: "PUT",
        url: `${API_URL_BASE}/CancelOrder/${id}`,
        success: function (data) {
            alert("Cancel Order Successfully");
            location.reload();
        },
        error: function (e) {
            alert(e.responseJSON.message);
        }
    });
};