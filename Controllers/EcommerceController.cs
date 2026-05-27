using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SwiftEcom.Models;
using SwiftEcom.Services;
using System.Security.Claims;
using System.Web.Http.Cors;

namespace SwiftEcom.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class EcommerceController : ApiController
    {
        EcommerceService _service = new EcommerceService();



        private string GetCustomerId()
        {
            var identity = User.Identity as ClaimsIdentity;
            return identity?.FindFirst("customerID")?.Value;
        }

        // ========================
        // AUTH
        // ========================

        // GET /api/stores
        [HttpGet]
        [Route("api/stores")]
        public IHttpActionResult GetStores()
        {
            SwiftEntities db = new SwiftEntities();
            var stores = db.Stores
                .Where(s => s.MyCompany == "019cce92-f61c-45b1")
                .Select(s => new { s.ID, s.StoreName })
                .ToList();
            return Ok(stores);
        }

        // GET /api/stores/search?name=abc
        [HttpGet]
        [Route("api/stores/search")]
        public IHttpActionResult SearchStores(string name)
        {
            using (SwiftEntities db = new SwiftEntities())
            {
                var stores = db.Stores
                    .Where(s => s.StoreName.Contains(name))
                    .Select(s => new { s.ID, s.StoreName })
                    .ToList();

                return Ok(stores);
            }
        }


        // POST /auth/signup
        [HttpPost]
        [Route("auth/signup")]
        public IHttpActionResult Signup([FromBody] SignupDTO model)
        {
            var result = _service.Signup(model);
            return Ok(result);
        }

        // POST /auth/login
        [HttpPost]
        [Route("auth/login")]

        public IHttpActionResult Login([FromBody] LoginDTO model)
        {
            var result = _service.Login(model);
            return Ok(result);
        }

        // POST /auth/logout
        [HttpPost]
        [Route("auth/logout")]
        public IHttpActionResult Logout()
        {
            var result = _service.Logout();
            return Ok(result);
        }

        // GET /auth/profile
        [HttpGet]
        [Route("auth/profile")]
        [Authorize]
        public IHttpActionResult GetProfile()
        {
            var customerId = GetCustomerId();
            var result = _service.GetProfile(customerId);
            return Ok(result);
        }

        // PUT /auth/profile/update
        [HttpPut]
        [Route("auth/profile/update")]
        [Authorize]
        public IHttpActionResult UpdateProfile([FromBody] UpdateProfileDTO model)
        {
            model.CustomerID = GetCustomerId();
            var result = _service.UpdateProfile(model);
            return Ok(result);
        }

        // PUT /auth/change-password
        [HttpPut]
        [Route("auth/change-password")]
        [Authorize]
        public IHttpActionResult ChangePassword([FromBody] ChangePasswordDTO model)
        {
            var customerID = GetCustomerId(); // from JWT token
            var result = _service.ChangePassword(customerID, model.OldPassword, model.NewPassword);
            return Ok(result);
        }


        // ========================
        // PRODUCTS
        // ========================

        // GET /products
        [HttpGet]
        [Route("products")]
        [Authorize]
        public IHttpActionResult GetAllProducts(int page = 1, int limit = 20)
        {
            var result = _service.GetAllProducts(page, limit);
            return Ok(result);
        }

        // GET /products/{id}
        [HttpGet]
        [Route("products/{id}")]
        [Authorize]
        public IHttpActionResult GetProductByID(string id)
        {
            var result = _service.GetProductByID(id);
            return Ok(result);
        }

        // GET /products/featured
        [HttpGet]
        [Route("products/featured")]
        [Authorize]
        public IHttpActionResult GetFeaturedProducts()
        {
            var result = _service.GetFeaturedProducts();
            return Ok(result);
        }

        // GET /products/category/{categoryId}
        [HttpGet]
        [Route("products/category/{categoryId}")]
        [Authorize]
        public IHttpActionResult GetProductsByCategory(string categoryId)
        {
            var result = _service.GetProductsByCategory(categoryId);
            return Ok(result);
        }

        // GET /products/subcategory/{subcategoryId}
        [HttpGet]
        [Route("products/subcategory/{subcategoryId}")]
        [Authorize]
        public IHttpActionResult GetProductsBySubcategory(string subcategoryId)
        {
            var result = _service.GetProductsBySubcategory(subcategoryId);
            return Ok(result);
        }

        // GET /products/{id}/variants
        [HttpGet]
        [Route("products/{id}/variants")]
        [Authorize]
        public IHttpActionResult GetProductVariants(string id)
        {
            var result = _service.GetProductVariants(id);
            return Ok(result);
        }

        // GET /products/{id}/variants/{variantId}
        [HttpGet]
        [Route("products/{id}/variants/{variantId}")]
        [Authorize]
        public IHttpActionResult GetVariantByID(string id, string variantId)
        {
            var result = _service.GetVariantByID(id, variantId);
            return Ok(result);
        }

        [HttpGet]
        [Route("products/{id}/images")]
        [Authorize]
        public IHttpActionResult GetImagesByProduct(string id)
        {
            var result = _service.GetImagesByProduct(id);
            return Ok(result);
        }

        [HttpGet]
        [Route("products/{id}/variants/{variantId}/images")]
        [Authorize]
        public IHttpActionResult GetImagesByVariant(string id, string variantId)
        {
            var result = _service.GetImagesByVariant(variantId);
            return Ok(result);
        }


        // ========================
        // CATEGORIES
        // ========================

        // GET /categories
        [HttpGet]
        [Route("categories")]
        [Authorize]
        public IHttpActionResult GetAllCategories()
        {
            var result = _service.GetAllCategories();
            return Ok(result);
        }

        // GET /categories/{id}
        [HttpGet]
        [Route("categories/{id}")]
        [Authorize]
        public IHttpActionResult GetCategoryByID(string id)
        {
            var result = _service.GetCategoryByID(id);
            return Ok(result);
        }


        // ========================
        // SUBCATEGORIES
        // ========================

        // GET /subcategories
        [HttpGet]
        [Route("subcategories")]
        [Authorize]
        public IHttpActionResult GetAllSubcategories()
        {
            var result = _service.GetAllSubcategories();
            return Ok(result);
        }

        // GET /subcategories/{id}
        [HttpGet]
        [Route("subcategories/{id}")]
        [Authorize]
        public IHttpActionResult GetSubcategoryByID(string id)
        {
            var result = _service.GetSubcategoryByID(id);
            return Ok(result);
        }

        // GET /subcategories/category/{categoryId}
        [HttpGet]
        [Route("subcategories/category/{categoryId}")]
        [Authorize]
        public IHttpActionResult GetSubcategoriesByCategory(string categoryId)
        {
            var result = _service.GetSubcategoriesByCategory(categoryId);
            return Ok(result);
        }


        // ========================
        // SEARCH
        // ========================

        // GET /search?query=
        [HttpGet]
        [Route("search")]
        [Authorize]
        public IHttpActionResult Search(string query = "", string category = "", decimal minPrice = 0, decimal maxPrice = 0)
        {
            var result = _service.Search(query, category, minPrice, maxPrice);
            return Ok(result);
        }

        // ========================
        // CART
        // ========================

        // GET /cart
        [HttpGet]
        [Route("cart")]
        [Authorize]
        public IHttpActionResult GetCart()
        {
            var customerId = GetCustomerId();

            var result = _service.GetCart(customerId);
            return Ok(result);
        }

        public class AddCartRequest
        {
            public string ProductID { get; set; }
            public int Quantity { get; set; }
        }

        // POST /cart/add
        [HttpPost]
        [Route("cart/add")]
        [Authorize]
        public IHttpActionResult AddToCart([FromBody] AddCartRequest model)
        {
            var customerId = GetCustomerId();

            var result = _service.AddToCart(customerId, model.ProductID, model.Quantity);
            return Ok(result);
        }

        public class UpdateCartRequest
        {
            public string ProductID { get; set; }
            public int Quantity { get; set; }
        }

        // PUT /cart/update
        [HttpPut]
        [Route("cart/update")]
        [Authorize]

        public IHttpActionResult UpdateCartItem([FromBody] UpdateCartRequest model)
        {
            var customerId = GetCustomerId();
            var result = _service.UpdateCartItem(customerId, model.ProductID, model.Quantity);
            return Ok(result);
        }

        public class RemoveCartRequest
        {
            public List<string> ProductIDs { get; set; }
        }

        // DELETE /cart/remove
        [HttpDelete]
        [Route("cart/remove")]
        [Authorize]
        public IHttpActionResult RemoveFromCart([FromBody] RemoveCartRequest model)
        {
            var customerId = GetCustomerId();

            var result = _service.RemoveFromCart(customerId, model.ProductIDs);
            return Ok(result);
        }

        // DELETE /cart/clear
        [HttpDelete]
        [Route("cart/clear")]
        [Authorize]
        public IHttpActionResult ClearCart()
        {
            var customerId = GetCustomerId();

            var result = _service.ClearCart(customerId);
            return Ok(result);
        }

        // ========================
        // SHIPPING
        // ========================

        // GET /customer/shipping
        [HttpGet]
        [Route("customer/shipping")]
        [Authorize]
        public IHttpActionResult GetShipping()
        {
            var customerId = GetCustomerId();

            var result = _service.GetShippingInfo(customerId);
            return Ok(result);
        }

        // PUT /customer/shipping
        [HttpPut]
        [Route("customer/shipping")]
        [Authorize]
        public IHttpActionResult UpdateShipping([FromBody] ShippingDTO model)
        {
            var customerId = GetCustomerId();

            var result = _service.UpsertShippingInfo(customerId, model);
            return Ok(result);
        }

        // ========================
        // CHECKOUT
        // ========================

        // POST /checkout
        [HttpPost]
        [Route("checkout")]
        [Authorize]
        public IHttpActionResult Checkout([FromBody] CheckoutDTO model)
        {
            var customerId = GetCustomerId();

            var result = _service.Checkout(customerId, model);
            return Ok(result);
        }

        // ========================
        // ORDER CONTROLLER ENDPOINTS
        // ========================

        // GET /orders
        [HttpGet]
        [Route("orders")]
        [Authorize]
        public IHttpActionResult GetOrders()
        {
            var customerID = GetCustomerId();
            var result = _service.GetOrders(customerID);
            return Ok(result);
        }

        // GET /orders/{invoiceID}
        [HttpGet]
        [Route("orders/{invoiceID}")]
        [Authorize]
        public IHttpActionResult GetOrderDetails(string invoiceID)
        {
            var customerID = GetCustomerId();
            var result = _service.GetOrderDetails(invoiceID, customerID);
            return Ok(result);
        }

        // PUT /orders/{invoiceID}/cancel
        [HttpPut]
        [Route("orders/{invoiceID}/cancel")]
        [Authorize]
        public IHttpActionResult CancelOrder(string invoiceID)
        {
            var customerID = GetCustomerId();
            var result = _service.CancelOrder(invoiceID, customerID);
            return Ok(result);
        }

    }
}