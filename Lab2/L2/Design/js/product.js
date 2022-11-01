const PRODUCT_API_URL = "http://localhost:5000/api"

function LoadProduct() {
    $.ajax({
        url: `${PRODUCT_API_URL}/Products/getAllProduct`,
        type: 'GET',
        success: function (rs) {
            $("#orders tbody tr").remove();
                var str = ` <tr>
                                  <th>ProductID</th>
                                  <th>ProductName</th>
                                  <th>UnitPrice</th>
                                  <th>Unit</th>
                                  <th>UnitsInStock</th>
                                  <th>Category</th>
                                  <th>Discontinued</th>
                                  <th></th>
                                </tr>`;
            $.each(rs, function (i, item) {
                str += "<tr>";
                str += "<th scope='row'>" + item.productId + "</th>";
                str += "<td>" + item.productName + "</td>";
                str += "<td>" + item.unitPrice + "</td>";
                str += "<td>" + item.quantityPerUnit + "</td>";
                str += "<td>" + item.unitsInStock + "</td>";
                str += "<td>" + item.category.categoryName + "</td>";
                str += "<td>" + item.discontinued + "</td>";
                str += `<td>
                    <a href="edit.html?id=${item.ProductId}">Edit</a> &nbsp; | &nbsp;
                    <a href="#" onclick="DeleteProduct(${item.ProductId}); return false;">Delete</a>
                </td>`;
                str += "</tr>"
            }); 
            $("#product").append(str);
        }
    });
}

function LoadCategory(){
    $.ajax({
        url: `${PRODUCT_API_URL}/Categories/ListCategory/`,
        type: 'GET',
        success: function (rs) {
            $('select[name="ddlCategory"]').empty();
                       $('select[name="ddlCategory"]').append('<option value="0">---Select---</option>');
                       $.each(rs, function(key, value) {
                           $('select[name="ddlCategory"]').append($('<option>').text(value.categoryName).attr('value', value.categoryId)).trigger("chosen:updated");            
                       });
        }
    });
}

function AddProduct(){
    $("#addProductForm").submit(function(event) {
                event.preventDefault();
                var catId = $('select[name="ddlCategory"]').val();;
                var data = {
                    ["ProductName"]: document.getElementById("ProductName").value,
                    ["UnitPrice"]: document.getElementById("UnitPrice").value,
                    ["UnitsInStock"]: document.getElementById("UnitsInStock").value,
                    ["Customer.Address"]: document.getElementById("Customer_Address").value,
                    CategoryId: catId,
                    QuantityPerUnit: document.getElementById("QuantityPerUnit").value,
                    Discontinued: document.querySelector('.messageCheckbox').checked,
                };

                $.ajax({
                    type: "POST",
                    enctype: 'multipart/form-data',
                    url: `${PRODUCT_API_URL}/products/create`,
                    data: data,
                    success: function (data) {
                        alert("Product Added Successfully");
                    },
                    error: function (e) {
                        alert(e.responseJSON.message);
                    }
                });
            }
)};

function SearchProduct(){
    var name = document.getElementById("txtSearch").value;
    var categoryId = $('select[name="ddlCategory"]').val();

    $.ajax({
        url: `${PRODUCT_API_URL}/products/search?name=${name}&category=${categoryId}`,
        type: 'GET',
        success: function (rs) {
            $("#product tr").remove();
            var str = ` <tr>
                              <th>ProductID</th>
                              <th>ProductName</th>
                              <th>UnitPrice</th>
                              <th>Unit</th>
                              <th>UnitsInStock</th>
                              <th>Category</th>
                              <th>Discontinued</th>
                              <th></th>
                            </tr>`;
            $.each(rs, function (i, item) {
                str += "<tr>";
                str += "<th scope='row'>" + item.productId + "</th>";
                str += "<td>" + item.productName + "</td>";
                str += "<td>" + item.unitPrice + "</td>";
                str += "<td>" + item.quantityPerUnit + "</td>";
                str += "<td>" + item.unitsInStock + "</td>";
                str += "<td>" + item.category.categoryName + "</td>";
                str += "<td>" + item.discontinued + "</td>";
                str += `<td>
                    <a href="edit.html?id=${item.ProductId}">Edit</a> &nbsp; | &nbsp;
                    <a href="#" onclick="DeleteProduct(${item.ProductId}); return false;">Delete</a>
                </td>`;
                str += "</tr>"
            }); 
            $("#product").append(str);
        }
    });
}

function EditProduct(){
    $("#editProductForm").submit(function (event) {
                event.preventDefault();
                var catId = $('select[name="ddlCategory"]').val();;
                var data = {
                    ["ProductId"]: document.getElementById("ProductId").value,
                    ["ProductName"]: document.getElementById("ProductName").value,
                    ["UnitPrice"]: document.getElementById("UnitPrice").value,
                    ["UnitsInStock"]: document.getElementById("UnitsInStock").value,
                    ["Customer.Address"]: document.getElementById("Customer_Address").value,
                    CategoryId: catId,
                    QuantityPerUnit: document.getElementById("QuantityPerUnit").value,
                    Discontinued: document.querySelector('.messageCheckbox').checked,
                };

                $.ajax({
                    type: "PUT",
                    enctype: 'multipart/form-data',
                    url: `${PRODUCT_API_URL}/products/edit`,
                    data: data,
                    success: function (data) {
                        alert("Product Updated Successfully");
                    },
                    error: function (e) {
                        alert(e.responseJSON.message);
                    }
                });
            }
)};

function DeleteProduct(id){
                $.ajax({
                    type: "DELETE",
                    url: `https://localhost:5000/api/products/delete/${id}`,
                    success: function (data) {
                        alert("Product Deleted Successfully");
                        location.reload();
                    },
                    error: function (e) {
                        alert(e.responseJSON.message);
                    }
                });
};