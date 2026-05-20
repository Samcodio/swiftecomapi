using SwiftEcom.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Helpers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace SwiftEcom.Services
{
    // ========================
    // DTOs
    // ========================

    public class SignupDTO
    {
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Address { get; set; }
    }

    public class LoginDTO
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class ChangePasswordDTO
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }

    public class LoginResponseDTO
    {
        public string Token { get; set; }
        public CustomerDTO Customer { get; set; }
    }

    public class UpdateProfileDTO
    {
        public string CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string PrefferedAddress { get; set; }
        public string PrefferedState { get; set; }
    }

    public class CustomerDTO
    {
        public string CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string PrefferedAddress { get; set; }
        public string PrefferedState { get; set; }
        public string Username { get; set; }
        public string ActiveStatus { get; set; }
        public DateTime? DOB { get; set; }
    }

    public class ProductDTO
    {
        public string ID { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public decimal? OnlineRate { get; set; }
        public decimal? SellingPrice { get; set; }
        public decimal? PercentOff { get; set; }
        public string LargeImage { get; set; }
        public string SmallImage { get; set; }
        public string Category { get; set; }
        public string Units { get; set; }
        public decimal? StockLevel { get; set; }
        public string Status { get; set; }
        public string ActiveStatus { get; set; }
        public string Features { get; set; }
        public string BarCode { get; set; }
        public List<VariantDTO> Variants { get; set; }
    }

    public class VariantDTO
    {
        public string ID { get; set; }
        public string Attribute { get; set; }
        public string Value { get; set; }
        public decimal? Rate { get; set; }
        public decimal? OnlineRate { get; set; }
        public decimal? Qty { get; set; }
        public decimal? StockLevel { get; set; }
        public string Status { get; set; }
        public string Code { get; set; }
        public string ProductID { get; set; }
    }

    public class CategoryDTO
    {
        public string ID { get; set; }
        public string Category { get; set; }
        public string ForSale { get; set; }
        public string MyCompany { get; set; }
    }

    public class SubcategoryDTO
    {
        public string ID { get; set; }
        public string Category { get; set; }
        public string ForSale { get; set; }
        public string MasterID { get; set; }
        public string MyCompany { get; set; }
    }

    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }

    public class ProductImageDTO
    {
        public string ID { get; set; }
        public string ProductID { get; set; }
        public string VariantID { get; set; }
        public string ImageName { get; set; }
        public string ImagePath { get; set; }
        public string FullImageUrl { get; set; } // ImagePath + ImageName combined
        public bool? IsFeatured { get; set; }
        public DateTime? DateCreated { get; set; }
    }

    public class CartDTO
    {
        public string CartID { get; set; }
        public List<CartItemDTO> Items { get; set; }
        public decimal Total { get; set; }
    }

    public class CartItemDTO
    {
        public string ProductID { get; set; }
        public string ProductName { get; set; }
        public decimal Quantity { get; set; }
        public decimal Rate { get; set; }
        public decimal Amount { get; set; }
        public string ImageUrl { get; set; }
    }

    public class CheckoutDTO
    {
        public string Notes { get; set; }
    }

    public class ShippingDTO
    {
        public string NameOfUser { get; set; }
        public string Address { get; set; }
        public string PhoneNo { get; set; }
        public string StateOfResidence { get; set; }
        public string EmailAddress { get; set; }
    }

    public class InvoiceDTO
    {
        public string InvoiceID { get; set; }
        public string CustomerID { get; set; }
        public DateTime Date { get; set; }
        public decimal Total { get; set; }
        public string Address { get; set; }
        public string Notes { get; set; }

        public List<InvoiceItemDTO> Items { get; set; }
    }

    public class InvoiceItemDTO
    {
        public string ProductID { get; set; }
        public string ProductName { get; set; }
        public decimal Quantity { get; set; }
        public decimal Rate { get; set; }
        public decimal Amount { get; set; }
    }

    public class OrderDTO
    {
        public string InvoiceID { get; set; }
        public string CustomerID { get; set; }
        public DateTime? Date { get; set; }
        public decimal? Total { get; set; }
        public decimal? AmountPaid { get; set; }
        public string Currency { get; set; }
        public string Address { get; set; }
        public string Notes { get; set; }
        public int? Status { get; set; }
        public int? Paid { get; set; }
        public string Ref { get; set; }
        public List<OrderItemDTO> Items { get; set; }
    }

    public class OrderItemDTO
    {
        public string ID { get; set; }
        public string ProductID { get; set; }
        public string ProductName { get; set; }
        public decimal? Qty { get; set; }
        public decimal? Rate { get; set; }
        public decimal? Amount { get; set; }
    }


    public class EcommerceService
    {
        SwiftEntities db = new SwiftEntities();

        public static class TenantContext
        {
            public const string CompanyId = "019cce92-f61c-45b1";
            public const string StoreId = "1B9A70-3X05HPX2";
        }


        // ========================
        // AUTH
        // ========================

        private string GenerateToken(string customerID, string username)
        {
            var secretKey = System.Configuration.ConfigurationManager.AppSettings["JwtSecretKey"];
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            var credentials = new SigningCredentials(
                securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("customerID", customerID),
                new Claim("username", username),
            };

            var token = new JwtSecurityToken(
                issuer: "SwiftEcom",
                audience: "SwiftEcom",
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string username, string password)
        {
            db.Configuration.ProxyCreationEnabled = false;
            string accPassword = db.Customers.Where(u => u.Username == username).Select(u => u.Password).FirstOrDefault();
            if (accPassword == null) return false;

            return BCrypt.Net.BCrypt.Verify(password, accPassword);
        }

        public ApiResponse<CustomerDTO> Signup(SignupDTO model)
        {
            try
            {
                // Check if username or email already exists
                var exists = db.Customers.Any(c => c.Username == model.Username || c.Email == model.Email);
                if (exists)
                    return new ApiResponse<CustomerDTO> { Success = false, Message = "Username or Email already exists" };

                var customer = new Customer
                {
                    CustomerID = Guid.NewGuid().ToString(),
                    CustomerName = model.CustomerName,
                    Email = model.Email,
                    Phone = model.Phone,
                    Username = model.Username,
                    Password = HashPassword(model.Password), 
                    Address = model.Address,
                    Date = DateTime.Now,
                    MyStore = TenantContext.StoreId,
                    ActiveStatus = "Active"
                };

                db.Customers.Add(customer);
                db.SaveChanges();

                return new ApiResponse<CustomerDTO>
                {
                    Success = true,
                    Message = "Registration successful",
                    Data = MapCustomerToDTO(customer)
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<CustomerDTO> { Success = false, Message = ex.Message };
            }
        }

        public ApiResponse<LoginResponseDTO> Login(LoginDTO model)
        {
            try
            {
                var customer = db.Customers.FirstOrDefault(c => c.Username == model.Username &&
                                db.Stores.Any(s => s.ID == c.MyStore && s.ID == TenantContext.StoreId));
                if (customer == null)
                    return new ApiResponse<LoginResponseDTO> { Success = false, Message = "Invalid username or password" };

                bool passwordMatch = BCrypt.Net.BCrypt.Verify(model.Password, customer.Password);
                if (!passwordMatch)
                    return new ApiResponse<LoginResponseDTO> { Success = false, Message = "Invalid username or password" };


                if (customer.ActiveStatus != "Active")
                    return new ApiResponse<LoginResponseDTO> { Success = false, Message = "Account is inactive" };

                string token = GenerateToken(customer.CustomerID, customer.Username);

                return new ApiResponse<LoginResponseDTO>
                {
                    Success = true,
                    Message = "Login successful",
                    Data = new LoginResponseDTO
                    {
                        Token = token,
                        Customer = MapCustomerToDTO(customer)
                    }
         
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<LoginResponseDTO> { Success = false, Message = ex.Message };
            }
        }
            
        public ApiResponse<string> Logout()
        {
            // Handle token invalidation here if using JWT
            return new ApiResponse<string> { Success = true, Message = "Logged out successfully" };
        }

        public ApiResponse<string> ChangePassword(string customerID, string oldPassword, string newPassword)
        {
            try
            {
                var customer = db.Customers.FirstOrDefault(c => c.CustomerID == customerID &&
                    db.Stores.Any(s =>
                        s.ID == c.MyStore && s.ID == TenantContext.StoreId));
                            if (customer == null)
                    return new ApiResponse<string> { Success = false, Message = "Customer not found" };

                // Verify old password
                bool passwordMatch = BCrypt.Net.BCrypt.Verify(oldPassword, customer.Password);
                if (!passwordMatch)
                    return new ApiResponse<string> { Success = false, Message = "Old password is incorrect" };

                // Hash and save new password
                customer.Password = HashPassword(newPassword);
                db.SaveChanges();

                return new ApiResponse<string> { Success = true, Message = "Password changed successfully" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<string> { Success = false, Message = ex.Message };
            }
        }

        public ApiResponse<CustomerDTO> GetProfile(string customerID)
        {
            try
            {
                var customer = db.Customers.FirstOrDefault(c => c.CustomerID == customerID &&
                db.Stores.Any(s =>
                    s.ID == c.MyStore && s.ID == TenantContext.StoreId));
                if (customer == null)
                    return new ApiResponse<CustomerDTO> { Success = false, Message = "Customer not found" };

                return new ApiResponse<CustomerDTO>
                {
                    Success = true,
                    Message = "Profile retrieved",
                    Data = MapCustomerToDTO(customer)
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<CustomerDTO> { Success = false, Message = ex.Message };
            }
        }

        public ApiResponse<CustomerDTO> UpdateProfile(UpdateProfileDTO model)
        {
            try
            {
                var customer = db.Customers.FirstOrDefault(c => c.CustomerID == model.CustomerID &&
                db.Stores.Any(s =>
                    s.ID == c.MyStore && s.ID == TenantContext.StoreId));
                if (customer == null)
                    return new ApiResponse<CustomerDTO> { Success = false, Message = "Customer not found" };

                customer.CustomerName = model.CustomerName;
                customer.Email = model.Email;
                customer.Phone = model.Phone;
                customer.Address = model.Address;
                customer.Address2 = model.Address2;
                customer.PrefferedAddress = model.PrefferedAddress;
                customer.PrefferedState = model.PrefferedState;

                db.SaveChanges();

                return new ApiResponse<CustomerDTO>
                {
                    Success = true,
                    Message = "Profile updated",
                    Data = MapCustomerToDTO(customer)
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<CustomerDTO> { Success = false, Message = ex.Message };
            }
        }


        // ========================
        // PRODUCTS
        // ========================

        public ApiResponse<List<ProductDTO>> GetAllProducts(int page, int limit)
        {
            try
            {
                var products = db.Products
                    .Where(p => p.ActiveStatus == "Active" && p.Status == "Active" &&
                    db.Stores.Any(s =>
                        s.ID == p.MyStore && s.ID == TenantContext.StoreId))
                    .OrderBy(p => p.Product1)
                    .Skip((page - 1) * limit)
                    .Take(limit)
                    .ToList()
                    .Select(p => MapProductToDTO(p))
                    .ToList();

                return new ApiResponse<List<ProductDTO>> { Success = true, Message = "Products retrieved", Data = products };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<ProductDTO>> { Success = false, Message = ex.Message };
            }
        }

        public ApiResponse<ProductDTO> GetProductByID(string id)
        {
            try
            {
                var product = db.Products.FirstOrDefault(p => p.ID == id &&
                    db.Stores.Any(s =>
                        s.ID == p.MyStore && s.ID == TenantContext.StoreId));
                if (product == null)
                    return new ApiResponse<ProductDTO> { Success = false, Message = "Product not found" };

                return new ApiResponse<ProductDTO> { Success = true, Message = "Product retrieved", Data = MapProductToDTO(product) };
            }
            catch (Exception ex)
            {
                return new ApiResponse<ProductDTO> { Success = false, Message = ex.Message };
            }
        }

        public ApiResponse<List<ProductDTO>> GetFeaturedProducts()
        {
            try
            {
                // Featured = active products with an online rate and a discount/percent off
                var products = db.Products
                .Where(p => p.ActiveStatus == "Active"
                    && p.Status == "Active"
                    && p.Features != null
                    && p.Features != ""
                     && db.Stores.Any(s => s.ID == p.MyStore && s.ID == TenantContext.StoreId))
                .ToList()
                .Select(p => MapProductToDTO(p))
                .ToList();

                return new ApiResponse<List<ProductDTO>> { Success = true, Message = "Featured products retrieved", Data = products };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<ProductDTO>> { Success = false, Message = ex.Message };
            }
        }

        public ApiResponse<List<ProductDTO>> GetProductsByCategory(string categoryId)
        {
            try
            {
                // Get category name from ProductCategoryMaster first
                var category = db.ProductCategoryMasters.FirstOrDefault(c => c.ID == categoryId && c.MyCompany == TenantContext.CompanyId);
                if (category == null)
                    return new ApiResponse<List<ProductDTO>> { Success = false, Message = "Category not found" };

                var subcategoryIDs = db.ProductCategories
                .Where(c => c.MasterID == categoryId && c.Company.ID == TenantContext.CompanyId)
                .Select(c => c.ID)
                .ToList();

                var products = db.Products
                    .Where(p => subcategoryIDs.Contains(p.Category) && p.ActiveStatus == "Active" && db.Stores.Any(s => s.ID == p.MyStore && s.ID == TenantContext.StoreId))
                    .ToList()
                    .Select(p => MapProductToDTO(p))
                    .ToList();

                return new ApiResponse<List<ProductDTO>> { Success = true, Message = "Products retrieved", Data = products };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<ProductDTO>> { Success = false, Message = ex.Message };
            }
        }

        public ApiResponse<List<ProductDTO>> GetProductsBySubcategory(string subcategoryId)
        {
            try
            {
                // Get subcategory from ProductCategory
                var subcategory = db.ProductCategories.FirstOrDefault(c => c.ID == subcategoryId && c.MyCompany == TenantContext.CompanyId);
                if (subcategory == null)
                    return new ApiResponse<List<ProductDTO>> { Success = false, Message = "Subcategory not found" };

                var products = db.Products
                    .Where(p => p.Category == subcategoryId && p.ActiveStatus == "Active" && db.Stores.Any(s => s.ID == p.MyStore && s.ID == TenantContext.StoreId))
                    .ToList()
                    .Select(p => MapProductToDTO(p))
                    .ToList();

                return new ApiResponse<List<ProductDTO>> { Success = true, Message = "Products retrieved", Data = products };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<ProductDTO>> { Success = false, Message = ex.Message };
            }
        }

        public ApiResponse<List<VariantDTO>> GetProductVariants(string productId)
        {
            try
            {
                var variants = db.ProductVariants
                    .Where(v => v.ProductID == productId && v.Status == "Active")
                    .ToList()
                    .Select(v => MapVariantToDTO(v))
                    .ToList();

                return new ApiResponse<List<VariantDTO>> { Success = true, Message = "Variants retrieved", Data = variants };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<VariantDTO>> { Success = false, Message = ex.Message };
            }
        }

        public ApiResponse<VariantDTO> GetVariantByID(string productId, string variantId)
        {
            try
            {
                var variant = db.ProductVariants.FirstOrDefault(v => v.ID == variantId && v.ProductID == productId);
                if (variant == null)
                    return new ApiResponse<VariantDTO> { Success = false, Message = "Variant not found" };

                return new ApiResponse<VariantDTO> { Success = true, Message = "Variant retrieved", Data = MapVariantToDTO(variant) };
            }
            catch (Exception ex)
            {
                return new ApiResponse<VariantDTO> { Success = false, Message = ex.Message };
            }
        }

        public ApiResponse<List<ProductImageDTO>> GetImagesByProduct(string productId)
        {
            try
            {
                var images = db.ProductImages
                    .Where(i => i.ProductID == productId)
                    .OrderByDescending(i => i.IsFeatured)
                    .ToList()
                    .Select(i => MapImageToDTO(i))
                    .ToList();

                return new ApiResponse<List<ProductImageDTO>>
                {
                    Success = true,
                    Message = "Images retrieved",
                    Data = images
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<ProductImageDTO>> { Success = false, Message = ex.Message };
            }
        }

        public ApiResponse<List<ProductImageDTO>> GetImagesByVariant(string variantId)
        {
            try
            {
                var images = db.ProductImages
                    .Where(i => i.VariantID == variantId)
                    .OrderByDescending(i => i.IsFeatured)
                    .ToList()
                    .Select(i => MapImageToDTO(i))
                    .ToList();

                return new ApiResponse<List<ProductImageDTO>>
                {
                    Success = true,
                    Message = "Images retrieved",
                    Data = images
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<ProductImageDTO>> { Success = false, Message = ex.Message };
            }
        }


        // ========================
        // CATEGORIES
        // ========================

        public ApiResponse<List<CategoryDTO>> GetAllCategories()
        {
            try
            {
                var categories = db.ProductCategoryMasters
                    .Where(c => c.MyCompany == TenantContext.CompanyId)
                    .Select(c => new CategoryDTO
                    {
                        ID = c.ID,
                        Category = c.Category,
                        ForSale = c.ForSale,
                        MyCompany = c.MyCompany,
                    })
                    .ToList();

                return new ApiResponse<List<CategoryDTO>> { Success = true, Message = "Categories retrieved", Data = categories };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<CategoryDTO>> { Success = false, Message = ex.Message };
            }
        }

        public ApiResponse<CategoryDTO> GetCategoryByID(string id)
        {
            try
            {
                var category = db.ProductCategoryMasters.FirstOrDefault(c => c.ID == id && c.MyCompany == TenantContext.CompanyId);
                if (category == null)
                    return new ApiResponse<CategoryDTO> { Success = false, Message = "Category not found" };

                return new ApiResponse<CategoryDTO>
                {
                    Success = true,
                    Message = "Category retrieved",
                    Data = new CategoryDTO
                    {
                        ID = category.ID,
                        Category = category.Category,
                        ForSale = category.ForSale,
                        MyCompany = category.MyCompany
                    }
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<CategoryDTO> { Success = false, Message = ex.Message };
            }
        }


        // ========================
        // SUBCATEGORIES
        // ========================

        public ApiResponse<List<SubcategoryDTO>> GetAllSubcategories()
        {
            try
            {
                var subcategories = db.ProductCategories
                    .Where(c => c.MyCompany == TenantContext.CompanyId)
                    .Select(c => new SubcategoryDTO
                    {
                        ID = c.ID,
                        Category = c.Category,
                        ForSale = c.ForSale,
                        MasterID = c.MasterID,
                        MyCompany = c.MyCompany
                    }).ToList();

                return new ApiResponse<List<SubcategoryDTO>> { Success = true, Message = "Subcategories retrieved", Data = subcategories };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<SubcategoryDTO>> { Success = false, Message = ex.Message };
            }
        }

        public ApiResponse<SubcategoryDTO> GetSubcategoryByID(string id)
        {
            try
            {
                var subcategory = db.ProductCategories.FirstOrDefault(c => c.ID == id && c.MyCompany == TenantContext.CompanyId);
                if (subcategory == null)
                    return new ApiResponse<SubcategoryDTO> { Success = false, Message = "Subcategory not found" };

                return new ApiResponse<SubcategoryDTO>
                {
                    Success = true,
                    Message = "Subcategory retrieved",
                    Data = new SubcategoryDTO
                    {
                        ID = subcategory.ID,
                        Category = subcategory.Category,
                        ForSale = subcategory.ForSale,
                        MasterID = subcategory.MasterID,
                        MyCompany = subcategory.MyCompany
                    }
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<SubcategoryDTO> { Success = false, Message = ex.Message };
            }
        }

        public ApiResponse<List<SubcategoryDTO>> GetSubcategoriesByCategory(string categoryId)
        {
            try
            {
                var subcategories = db.ProductCategories
                    .Where(c => c.MasterID == categoryId && c.MyCompany == TenantContext.CompanyId)
                    .ToList()
                    .Select(c => new SubcategoryDTO
                    {
                        ID = c.ID,
                        Category = c.Category,
                        ForSale = c.ForSale,
                        MasterID = c.MasterID,
                        MyCompany = c.MyCompany
                    }).ToList();

                return new ApiResponse<List<SubcategoryDTO>> { Success = true, Message = "Subcategories retrieved", Data = subcategories };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<SubcategoryDTO>> { Success = false, Message = ex.Message };
            }
        }


        // ========================
        // SEARCH
        // ========================

        public ApiResponse<List<ProductDTO>> Search(string query, string category, decimal minPrice, decimal maxPrice)
        {
            try
            {
                var productsQuery = db.Products
                    .Where(p => p.ActiveStatus == "Active"
                        && db.Stores.Any(s =>
                            s.ID == p.MyStore && s.ID == TenantContext.StoreId));

                // Filter by search query
                if (!string.IsNullOrEmpty(query))
                {
                    productsQuery = productsQuery.Where(p =>
                        p.Product1.Contains(query) ||
                        p.Description.Contains(query) ||
                        p.Features.Contains(query) ||
                        p.BarCode.Contains(query)
                    );
                }

                // Filter by category
                if (!string.IsNullOrEmpty(category))
                {
                    productsQuery = productsQuery.Where(p => p.Category == category);
                }

                // Filter by price range
                if (minPrice > 0)
                {
                    productsQuery = productsQuery.Where(p => p.OnlineRate >= minPrice);
                }

                if (maxPrice > 0)
                {
                    productsQuery = productsQuery.Where(p => p.OnlineRate <= maxPrice);
                }

                var results = productsQuery
                    .ToList()
                    .Select(p => MapProductToDTO(p))
                    .ToList();

                return new ApiResponse<List<ProductDTO>> { Success = true, Message = $"{results.Count} result(s) found", Data = results };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<ProductDTO>> { Success = false, Message = ex.Message };
            }
        }

        // ========================
        // CART
        // ========================

        public ApiResponse<CartDTO> GetCart(string customerId)
        {
            try
            {
                var cart = db.Carts.FirstOrDefault(c => c.UserID == customerId && c.Status == 1);

                if (cart == null)
                {
                    cart = new Cart
                    {
                        ID = Guid.NewGuid().ToString(),
                        UserID = customerId,
                        Date = DateTime.Now,
                        Status = 1,
                    };

                    db.Carts.Add(cart);
                    db.SaveChanges();
                }

                var items = db.ShoppingCartDetails
                    .Where(x => x.MasterID == cart.ID)
                    .ToList()
                    .Select(MapCartItemToDTO)
                    .ToList();

                var total = items.Sum(x => x.Amount);

                return new ApiResponse<CartDTO>
                {
                    Success = true,
                    Message = "Cart retrieved",
                    Data = new CartDTO
                    {
                        CartID = cart.ID,
                        Items = items,
                        Total = total
                    }
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<CartDTO>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public ApiResponse<string> AddToCart(string customerId, string productId, int quantity)
        {
            try
            {
                var cart = db.Carts.FirstOrDefault(c => c.UserID == customerId);

                if (cart == null)
                {
                    cart = new Cart
                    {
                        ID = Guid.NewGuid().ToString(),
                        UserID = customerId,
                        Date = DateTime.Now,
                        Status = 1
                    };

                    db.Carts.Add(cart);
                    db.SaveChanges();
                }

                var existingItem = db.ShoppingCartDetails.FirstOrDefault(x => x.MasterID == cart.ID && x.ProductID == productId);

                if (existingItem != null)
                {
                    existingItem.Quantity += quantity;
                    existingItem.Amount = (existingItem.Rate ?? 0) * existingItem.Quantity;

                    db.SaveChanges();

                    return new ApiResponse<string>
                    {
                        Success = true,
                        Message = "Cart item quantity updated"
                    };
                }

                var product = db.Products.FirstOrDefault(p => p.ID == productId &&
                db.Stores.Any(s =>
                    s.ID == p.MyStore && s.ID == TenantContext.StoreId));

                if (product == null)
                {
                    return new ApiResponse<string>
                    {
                        Success = false,
                        Message = "Product not found"
                    };
                }

                var item = new ShoppingCartDetail
                {
                    ID = Guid.NewGuid().ToString(),
                    MasterID = cart.ID,
                    ProductID = productId,
                    Quantity = quantity,
                    Rate = product.OnlineRate ?? 0,
                    Amount = (product.OnlineRate ?? 0) * quantity
                };

                db.ShoppingCartDetails.Add(item);
                db.SaveChanges();

                return new ApiResponse<string>
                {
                    Success = true,
                    Message = "Item added to cart"
                };
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                if (ex.InnerException != null)
                    message += " | " + ex.InnerException.Message;
                if (ex.InnerException?.InnerException != null)
                    message += " | " + ex.InnerException.InnerException.Message;

                return new ApiResponse<string>
                {
                    Success = false,
                    Message = message
                };
            }
        }

        public ApiResponse<string> UpdateCartItem(string customerId, string productId, int quantity)
        {
            try
            {
                var cart = db.Carts.FirstOrDefault(c => c.UserID == customerId && c.Status == 1);

                if (cart == null)
                {
                    return new ApiResponse<string>
                    {
                        Success = false,
                        Message = "Cart not found"
                    };
                }

                var item = db.ShoppingCartDetails.FirstOrDefault(x => x.MasterID == cart.ID && x.ProductID == productId);

                if (item == null)
                {
                    return new ApiResponse<string>
                    {
                        Success = false,
                        Message = "Item not found in cart"
                    };
                }

                if (quantity <= 0)
                {
                    db.ShoppingCartDetails.Remove(item);
                    db.SaveChanges();

                    return new ApiResponse<string>
                    {
                        Success = true,
                        Message = "Item removed due to zero quantity"
                    };
                }

                item.Quantity = quantity;
                item.Amount = (item.Rate ?? 0) * quantity;

                db.SaveChanges();

                return new ApiResponse<string>
                {
                    Success = true,
                    Message = "Cart item updated"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public ApiResponse<string> RemoveFromCart(string customerId, List<string> productIds)
        {
            try
            {
                var cart = db.Carts.FirstOrDefault(c => c.UserID == customerId && c.Status == 1);

                if (cart == null)
                {
                    return new ApiResponse<string>
                    {
                        Success = false,
                        Message = "Cart not found"
                    };
                }

                var items = db.ShoppingCartDetails
                    .Where(x => x.MasterID == cart.ID && productIds.Contains(x.ProductID))
                    .ToList();

                if (!items.Any())
                {
                    return new ApiResponse<string>
                    {
                        Success = false,
                        Message = "No matching items found"
                    };
                }

                db.ShoppingCartDetails.RemoveRange(items);
                db.SaveChanges();

                return new ApiResponse<string>
                {
                    Success = true,
                    Message = "Item(s) removed from cart"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public ApiResponse<string> ClearCart(string customerId)
        {
            try
            {
                var cart = db.Carts.FirstOrDefault(c => c.UserID == customerId && c.Status == 1);

                if (cart == null)
                {
                    return new ApiResponse<string>
                    {
                        Success = false,
                        Message = "Cart not found"
                    };
                }

                var items = db.ShoppingCartDetails
                    .Where(x => x.MasterID == cart.ID)
                    .ToList();

                db.ShoppingCartDetails.RemoveRange(items);
                db.SaveChanges();

                return new ApiResponse<string>
                {
                    Success = true,
                    Message = "Cart cleared"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        // ========================
        // SHIPPINGINFO
        // ========================

        public ApiResponse<ShippingInfo> GetShippingInfo(string customerId)
        {
            try
            {
                var shipping = db.ShippingInfoes
                    .FirstOrDefault(x => x.CustomerID == customerId);

                if (shipping == null)
                {
                    return new ApiResponse<ShippingInfo>
                    {
                        Success = false,
                        Message = "Shipping info not found",
                        Data = null
                    };
                }

                return new ApiResponse<ShippingInfo>
                {
                    Success = true,
                    Message = "Shipping info retrieved",
                    Data = shipping
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<ShippingInfo>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public ApiResponse<string> UpsertShippingInfo(string customerId, ShippingDTO model)
        {
            try
            {
                var shipping = db.ShippingInfoes
                    .FirstOrDefault(x => x.CustomerID == customerId);

                if (shipping == null)
                {
                    shipping = new ShippingInfo
                    {
                        ID = 0, // int identity assumed (based on DB)
                        CustomerID = customerId,
                        NameOfUser = model.NameOfUser,
                        Address = model.Address,
                        PhoneNo = model.PhoneNo,
                        StateOfResidence = model.StateOfResidence,
                        EmailAddress = model.EmailAddress,
                        Status = 1
                    };

                    db.ShippingInfoes.Add(shipping);
                }
                else
                {
                    shipping.NameOfUser = model.NameOfUser;
                    shipping.Address = model.Address;
                    shipping.PhoneNo = model.PhoneNo;
                    shipping.StateOfResidence = model.StateOfResidence;
                    shipping.EmailAddress = model.EmailAddress;
                }

                db.SaveChanges();

                return new ApiResponse<string>
                {
                    Success = true,
                    Message = "Shipping info saved successfully"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        // ========================
        // CHECKOUT
        // ========================

        public ApiResponse<InvoiceDTO> Checkout(string customerId, CheckoutDTO model)
        {
            try
            {
                var cart = db.Carts
                    .FirstOrDefault(c => c.UserID == customerId && c.Status == 1);

                if (cart == null)
                {
                    return new ApiResponse<InvoiceDTO>
                    {
                        Success = false,
                        Message = "Cart not found"
                    };
                }

                var cartItems = db.ShoppingCartDetails
                    .Where(x => x.MasterID == cart.ID)
                    .ToList();

                if (!cartItems.Any())
                {
                    return new ApiResponse<InvoiceDTO>
                    {
                        Success = false,
                        Message = "Cart is empty"
                    };
                }

                var shipping = db.ShippingInfoes
                    .FirstOrDefault(x => x.CustomerID == customerId);

                if (shipping == null || string.IsNullOrEmpty(shipping.Address))
                {
                    return new ApiResponse<InvoiceDTO>
                    {
                        Success = false,
                        Message = "Shipping information is required before checkout"
                    };
                }

                decimal total = cartItems.Sum(x => x.Amount ?? 0);

                string invoiceId = Guid.NewGuid().ToString();

                var invoice = new AccountInvoice
                {
                    ID = invoiceId,
                    MyCustomer = customerId,
                    Date = DateTime.Now,
                    Bill = total,
                    AmountPaid = 0,
                    Status = 1,
                    PostStatus = 0,
                    Address = shipping.Address,
                    Notes = model?.Notes,
                    Currency = "1010101",
                    myserial = db.AccountInvoices.Any()
                    ? db.AccountInvoices.Max(x => x.myserial) + 1
                    : 1
                };

                db.AccountInvoices.Add(invoice);

                var itemsDto = new List<InvoiceItemDTO>();

                foreach (var item in cartItems)
                {
                    var product = db.Products.FirstOrDefault(p => p.ID == item.ProductID &&
                            db.Stores.Any(s =>
                                s.ID == p.MyStore && s.ID == TenantContext.StoreId));

                    db.AccountInvoiceDetails.Add(new AccountInvoiceDetail
                    {
                        ID = Guid.NewGuid().ToString(),
                        MyInvoice = invoiceId,
                        ProductID = item.ProductID,
                        Qty = item.Quantity,
                        Rate = item.Rate,
                        Amount = item.Amount,
                        Username = customerId,
                        myserial = db.AccountInvoiceDetails.Any()
                            ? db.AccountInvoiceDetails.Max(x => x.myserial) + 1
                            : 1
                    });

                    itemsDto.Add(new InvoiceItemDTO
                    {
                        ProductID = item.ProductID,
                        ProductName = product?.Product1,
                        Quantity = item.Quantity ?? 0,
                        Rate = item.Rate ?? 0,
                        Amount = item.Amount ?? 0
                    });
                }

                db.ShoppingCartDetails.RemoveRange(cartItems);

                db.SaveChanges();

                var response = new InvoiceDTO
                {
                    InvoiceID = invoiceId,
                    CustomerID = customerId,
                    Date = DateTime.Now,
                    Total = total,
                    Address = shipping.Address,
                    Notes = model?.Notes,
                    Items = itemsDto
                };

                return new ApiResponse<InvoiceDTO>
                {
                    Success = true,
                    Message = "Checkout successful",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                if (ex.InnerException != null)
                    message += " | " + ex.InnerException.Message;
                if (ex.InnerException?.InnerException != null)
                    message += " | " + ex.InnerException.InnerException.Message;
                return new ApiResponse<InvoiceDTO>
                {
                    Success = false,
                    Message = message
                };
            }
        }


        // ========================
        // ORDER SERVICE METHODS
        // (Add these inside EcommerceService class)
        // ========================

        // List Orders
        public ApiResponse<List<OrderDTO>> GetOrders(string customerID)
        {
            try
            {
                var orders = (from i in db.AccountInvoices
                              join c in db.Customers on i.MyCustomer equals c.CustomerID
                              join s in db.Stores on c.MyStore equals s.ID
                              where i.MyCustomer == customerID
                                     && s.ID == TenantContext.StoreId
                              orderby i.Date descending
                              select new OrderDTO
                              {
                                  InvoiceID = i.ID,
                                  CustomerID = i.MyCustomer,
                                  Date = i.Date,
                                  Total = i.Bill ?? 0,
                                  AmountPaid = i.AmountPaid ?? 0,
                                  Currency = i.Currency,
                                  Address = i.Address,
                                  Notes = i.Notes,
                                  Status = i.Status,
                                  Paid = i.Paid,
                                  Ref = i.Ref,
                                  Items = null
                              }).ToList();

                return new ApiResponse<List<OrderDTO>>
                {
                    Success = true,
                    Message = "Orders retrieved",
                    Data = orders
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<OrderDTO>> { Success = false, Message = ex.Message };
            }
        }

        // View Order Details
        public ApiResponse<OrderDTO> GetOrderDetails(string invoiceID, string customerID)
        {
            try
            {
                var invoice = (from i in db.AccountInvoices
                               join c in db.Customers on i.MyCustomer equals c.CustomerID
                               join s in db.Stores on c.MyStore equals s.ID
                               where i.ID == invoiceID
                                     && i.MyCustomer == customerID
                                      && s.ID == TenantContext.StoreId
                               select i)
              .FirstOrDefault();

                if (invoice == null)
                    return new ApiResponse<OrderDTO> { Success = false, Message = "Order not found" };

                var rawItems = db.AccountInvoiceDetails
                    .Where(d => d.MyInvoice == invoiceID)
                    .ToList();

                var productIDs = rawItems.Select(d => d.ProductID).Distinct().ToList();
                var productMap = db.Products
                    .Where(p => productIDs.Contains(p.ID))
                    .ToDictionary(p => p.ID);

                var items = rawItems.Select(d => new OrderItemDTO
                {
                    ID = d.ID,
                    ProductID = d.ProductID,
                    ProductName = productMap.ContainsKey(d.ProductID)
                        ? productMap[d.ProductID].Product1
                        : "Unknown Product",
                    Qty = d.Qty ?? 0,
                    Rate = d.Rate ?? 0,
                    Amount = d.Amount ?? 0
                }).ToList();

                var order = new OrderDTO
                {
                    InvoiceID = invoice.ID,
                    CustomerID = invoice.MyCustomer,
                    Date = invoice.Date,
                    Total = invoice.Bill ?? 0,
                    AmountPaid = invoice.AmountPaid ?? 0,
                    Currency = invoice.Currency,
                    Address = invoice.Address,
                    Notes = invoice.Notes,
                    Status = invoice.Status,
                    Paid = invoice.Paid,
                    Ref = invoice.Ref,
                    Items = items
                };

                return new ApiResponse<OrderDTO>
                {
                    Success = true,
                    Message = "Order details retrieved",
                    Data = order
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<OrderDTO> { Success = false, Message = ex.Message };
            }
        }

        // Cancel Order
        public ApiResponse<string> CancelOrder(string invoiceID, string customerID)
        {
            try
            {
                var invoice = (from i in db.AccountInvoices
                               join c in db.Customers on i.MyCustomer equals c.CustomerID
                               join s in db.Stores on c.MyStore equals s.ID
                               where i.ID == invoiceID
                                     && i.MyCustomer == customerID
                                      && s.ID == TenantContext.StoreId
                               select i)
              .FirstOrDefault();

                if (invoice == null)
                    return new ApiResponse<string> { Success = false, Message = "Order not found" };

                // Check if already cancelled
                if (invoice.Status == 0)
                    return new ApiResponse<string> { Success = false, Message = "Order is already cancelled" };

                // Can't cancel a paid order
                if (invoice.Paid == 1)
                    return new ApiResponse<string> { Success = false, Message = "Cannot cancel a paid order" };

                // Can't cancel an order already being processed
                if (invoice.PostStatus == 1)
                    return new ApiResponse<string> { Success = false, Message = "Cannot cancel an order that is already being processed" };

                // Status: 0 = Cancelled, 1 = Active, 2 = Processing
                invoice.Status = 0;
                invoice.StatusNote = "Cancelled by customer";

                db.SaveChanges();

                return new ApiResponse<string>
                {
                    Success = true,
                    Message = "Order cancelled successfully"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<string> { Success = false, Message = ex.Message };
            }
        }

        // ========================
        // MAPPERS
        // ========================

        private CustomerDTO MapCustomerToDTO(Customer c)
        {
            return new CustomerDTO
            {
                CustomerID = c.CustomerID,
                CustomerName = c.CustomerName,
                Email = c.Email,
                Phone = c.Phone,
                Address = c.Address,
                Address2 = c.Address2,
                PrefferedAddress = c.PrefferedAddress,
                PrefferedState = c.PrefferedState,
                Username = c.Username,
                ActiveStatus = c.ActiveStatus,
                DOB = c.DOB
            };
        }

        private ProductDTO MapProductToDTO(Product p)
        {
            var variants = db.ProductVariants
                 .Where(v => v.ProductID == p.ID && v.Status == "Active")
                 .ToList()
                 .Select(v => MapVariantToDTO(v))
                 .ToList();

            // Get category name from ProductCategory table
            var categoryName = db.ProductCategories
                .Where(c => c.ID == p.Category)
                .Select(c => c.Category)
                .FirstOrDefault();

            return new ProductDTO
            {
                ID = p.ID,
                ProductName = p.Product1,
                Description = p.Description,
                OnlineRate = p.OnlineRate,
                SellingPrice = p.SellingPrice,
                PercentOff = p.PercentOff,
                LargeImage = p.LargeImage,
                SmallImage = p.SmallImage,
                Category = categoryName ?? p.Category,
                Units = p.Units,
                StockLevel = p.StockLevel,
                Status = p.Status,
                ActiveStatus = p.ActiveStatus,
                Features = p.Features,
                BarCode = p.BarCode,
                Variants = variants
            };
        }

        private VariantDTO MapVariantToDTO(ProductVariant v)
        {
            return new VariantDTO
            {
                ID = v.ID,
                Attribute = v.Attribute,
                Value = v.Value,
                Rate = v.Rate,
                OnlineRate = v.OnlineRate,
                Qty = v.Qty,
                StockLevel = v.StockLevel,
                Status = v.Status,
                Code = v.Code,
                ProductID = v.ProductID
            };
        }

        private ProductImageDTO MapImageToDTO(ProductImage i)
        {
            return new ProductImageDTO
            {
                ID = i.ID,
                ProductID = i.ProductID,
                VariantID = i.VariantID,
                ImageName = i.ImageName,
                ImagePath = i.ImagePath,
                FullImageUrl = i.ImagePath + i.ImageName, // combine for frontend
                IsFeatured = i.IsFeatured,
                DateCreated = i.DateCreated
            };
        }

        private CartItemDTO MapCartItemToDTO(ShoppingCartDetail x)
        {
            var product = db.Products.FirstOrDefault(p => p.ID == x.ProductID);

            return new CartItemDTO
            {
                ProductID = x.ProductID,
                ProductName = product?.Product1,
                Quantity = x.Quantity ?? 0,
                Rate = x.Rate ?? 0,
                Amount = x.Amount ?? 0,
                ImageUrl = product?.SmallImage
            };
        }
    }
}