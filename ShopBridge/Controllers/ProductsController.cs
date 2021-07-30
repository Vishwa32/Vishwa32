using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using ShopBridge.Data;
using ShopBridge.Models;

namespace ShopBridge.Controllers
{
    public class ProductsController : ApiController
    {
        private DataContext db = new DataContext();

        #region Response Codes and Its Description
        //100 : Record Not Found 
        //200 : Success 
        //300 : invalid input
        //400 : Already item present.
        //500 : Other errors /Server exceptions etc 
        #endregion


        #region List Of Product
        /// <summary>
        /// This method returns list of Product present in the Product table.
        /// </summary>
        /// <param name="page">number of page</param>
        /// <param name="pageSize">size of records </param>
        /// <returns>List Of Products</returns>

        // GET: api/Products
        public Status GetProduct(int page = 1, int pageSize = 1)
        {

            Status status = new Status();
            try
            {
                if (page < 0)
                {
                    status.RecordCount = 0;
                    status.StatusCode = "100";
                    status.StatusMessage = "Invalid Page number";

                    return status;
                }

                page = page - 1;

                if (db.Product.ToList().Count == 0)
                {
                    status.RecordCount = db.Product.ToList().Count;
                    status.StatusCode = "100";
                    status.StatusMessage = "Record(s) are not exists";
                    return status;
                }

                status.RecordCount = db.Product.ToList().Count;
                status.StatusCode = "200";
                status.StatusMessage = "Record(s) found successfully";
                status.Products = new List<Product>();

                var list = db.Product.OrderBy(i => i.ProductID).Skip(page * pageSize).Take(pageSize).ToList();

                foreach (var rows in list)
                {
                    Product product = new Product();
                    product.ProductID = string.IsNullOrEmpty(rows.ProductID.ToString()) == true ? 0 : rows.ProductID;
                    product.Description = string.IsNullOrEmpty(rows.Description.ToString()) == true ? "" : rows.Description;
                    product.Price = string.IsNullOrEmpty(rows.Price.ToString()) == true ? 0 : rows.Price; 
                    product.Name = string.IsNullOrEmpty(rows.Name.ToString()) == true ? "" : rows.Name;
                    status.Products.Add(product);
                }

                if (status.Products.Count==0)
                {
                    status.RecordCount = 0;
                    status.StatusCode = "100";
                    status.StatusMessage = "Invalid Page number";

                    return status;
                }
            }
            catch (Exception ex)
            {
                status.RecordCount = 0;
                status.StatusCode = "500";
                status.StatusMessage = ex.Message.ToString();
            }

            return status;
        }
        #endregion


        #region Single Produuct
        /// <summary>
        /// This method returns Product all details by its Id.
        /// </summary>
        /// <param name="id">Id is product id</param>
        /// <returns>Single exist Product</returns>
        // GET: api/Products/5
        [ResponseType(typeof(Product))]
        public StatusForSinle GetProduct(int id)
        {
            StatusForSinle status = new StatusForSinle();
            try
            {
                Product product = db.Product.Find(id);
                if (product == null)
                {
                    status.StatusMessage = "Record not found !!";
                    status.StatusCode = "100";
                    return status;
                }

                status.RecordCount = 1;
                status.StatusCode = "200";
                status.StatusMessage = "Record found successfully";
                status.Products = new Product();
                status.Products.Name = string.IsNullOrEmpty(product.Name.ToString()) == true ? "" : product.Name.ToString();
                status.Products.Price = string.IsNullOrEmpty(product.Price.ToString()) == true ? 0 : product.Price;
                status.Products.ProductID = string.IsNullOrEmpty(product.ProductID.ToString()) == true ? 0 : product.ProductID;
                status.Products.Description = string.IsNullOrEmpty(product.Description.ToString()) == true ? "" : product.Description;
            }
            catch (Exception ex)
            {
                status.RecordCount = 0;
                status.StatusCode = "500";
                status.StatusMessage = ex.Message.ToString();
            }


            return status;
        }
        #endregion


        #region Update Produuct
        /// <summary>
        /// This Method is used to Update already exist record.
        /// </summary>
        /// <param name="id">it is a id of product that you want to update</param>
        /// <param name="product"> this is a class which contains Product id,price,name and description </param>
        /// <returns>returns success if successfully update record</returns>
        //// PUT: api/Products/5
        [ResponseType(typeof(StatusSaveUpdate))]
        public StatusSaveUpdate PutProduct(int id, Product product)
        {
            StatusSaveUpdate status = new StatusSaveUpdate();

            if (!ModelState.IsValid)
            {
                status.ResponseCode = "004";

                int ProId = 0;
                string strProId = string.IsNullOrEmpty(product.ProductID.ToString()) == true ? "0" : product.ProductID.ToString();
                int.TryParse(strProId, out ProId);

                decimal ProductPrice = 0;
                string strProductPrice = string.IsNullOrEmpty(product.Price.ToString()) == true ? "0" : product.Price.ToString();
                decimal.TryParse(strProductPrice, out ProductPrice);

                if (ProId <= 0)
                {
                    status.ResponseMessage = "Please enter valid ProductID";
                }
                if (ProductPrice <= 0)
                {
                    status.ResponseMessage ="Please enter price greater than 0";
                }
                return status;
            }

            if (id != product.ProductID)
            {
                status.ResponseMessage = "Record with Id:" + id + " not found !!";
                status.ResponseCode = "100";
                return status;
            }

            db.Entry(product).State = EntityState.Modified;

            try
            {
                db.SaveChanges();

                status.ResponseMessage = "Record Updated Successfully.";
                status.ResponseCode = "200";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    status.ResponseMessage = "Record with Id:" + id + " not found !!";
                    status.ResponseCode = "100";
                    return status;
                }
                else
                {
                    throw;
                }
            }

            catch (Exception ex)
            {
                status.ResponseMessage = ex.Message;
                status.ResponseCode = "200";
                return status;
            }

            return status;
        }
        #endregion


        #region Insert Product
        /// <summary>
        /// This Method is used to Insert new record.
        /// </summary>
        /// <param name="product"> this is a class which contains Product id,price,name and description</param>
        /// <returns>returns success if successfully inserted record</returns>
        // POST: api/Products
        [ResponseType(typeof(StatusSaveUpdate))]
        public StatusSaveUpdate PostProduct(Product product)
        {
            StatusSaveUpdate status = new StatusSaveUpdate();
            try
            {
                if (!ModelState.IsValid)
                {
                    double price;

                    if (!double.TryParse(product.Price.ToString(), out price))
                    {
                        status.ResponseCode = "300";
                        status.ResponseMessage = "Please enter valid value for Price";
                        return status;
                    }
                }

                var dup = db.Product.Where(p => p.Name == product.Name).FirstOrDefault();
                if (dup == null)
                {
                    db.Product.Add(product);
                    db.SaveChanges();

                    status.ResponseCode = "200";
                    status.ResponseMessage = "Record Inserted Successfully.";

                    return status;
                }
                else
                {
                    status.ResponseCode = "400";
                    status.ResponseMessage = "Product With Name:" + dup.Name + " already exists !!";

                    return status;
                }
            }
            catch (Exception ex)
            {
                status.ResponseCode = "500";
                status.ResponseMessage = ex.Message;
            }
            return status;
        }
        #endregion


        #region Delete Product
        /// <summary>
        /// used to delete existing record from Product
        /// </summary>
        /// <param name="id">existing product id </param>
        /// <returns>returns success message if record deleted successfully</returns>
        // DELETE: api/Products/5
        [ResponseType(typeof(Product))]
        public StatusSaveUpdate DeleteProduct(int id)
        {
            StatusSaveUpdate status = new StatusSaveUpdate();
            try
            {
                Product product = db.Product.Find(id);
                if (product == null)
                {
                    status.ResponseMessage = "Record with Id:" + id + " not found !!";
                    status.ResponseCode = "002";
                    return status;
                }

                db.Product.Remove(product);
                db.SaveChanges();

                status.ResponseMessage = "Record with Id:" + id + " deleted successfully !!";
                status.ResponseCode = "200";

            }
            catch (Exception ex)
            {

                status.ResponseMessage = ex.Message;
                status.ResponseCode = "500";
                return status;
            }

            return status;
        }
        #endregion


        #region Check Product Exist or Not
        /// <summary>
        /// used to confirm product of given id is exist or not
        /// </summary>
        /// <param name="id">Id of the product</param>
        /// <returns>returns true if product exist else returns false</returns>
        private bool ProductExists(int id)
        {
            return db.Product.Count(e => e.ProductID == id) > 0;
        }
        #endregion
    }
}