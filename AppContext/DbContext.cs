using EcommerceHomework.Models;
using EcommerceHomework.Models.ViewModels;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EcommerceHomework.AppContext
{
    public class DbContext
    {
        public string ConnectionString { get; set; }

        public DbContext(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        private MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }

        public List<String> GetCategories()
        {
            List<String> categories = new List<string>();

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("select distinct category from item", conn);

                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var cat = reader["category"].ToString();
                    categories.Add(cat);
                }
            }

            return categories;
        }

        public List<Item> GetFilteredItems(string categoryName)
        {
            List<Item> list = new List<Item>();

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("select * from item where category=@categoryName", conn);
                cmd.Parameters.AddWithValue("@categoryName", categoryName);

                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    list.Add(new Item()
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Name = reader["Name"].ToString(),
                        Category = reader["Category"].ToString(),
                        Price = float.Parse(reader["Price"].ToString()),
                        Description = reader["Description"].ToString(),
                        Photo = (byte[])reader["photo"]
                    });
                }
            }
            return list;
        }

        public List<Item> GetAllItems()
        {
            List<Item> list = new List<Item>();

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("select * from item", conn);

                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    list.Add(new Item()
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Name = reader["Name"].ToString(),
                        Category = reader["Category"].ToString(),
                        Price = float.Parse(reader["Price"].ToString()),
                        Description = reader["Description"].ToString(),
                        Photo = (byte[])reader["photo"]
                    });
                }
            }
            return list;
        }

        public Item GetItem(int id)
        {
            Item item = null;
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("select * from item where id = '" + id + "'", conn);

                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    item = new Item()
                    {
                        Id = Convert.ToInt32(reader["id"]),
                        Name = reader["name"].ToString(),
                        Photo = (byte[])reader["photo"],
                        Category = reader["category"].ToString(),
                        Price = float.Parse(reader["price"].ToString()),
                        Description = reader["description"].ToString()
                    };
                }
            }
            return item;
        }

        public bool EditItem(int id, Item item)
        {
            using MySqlConnection conn = GetConnection();

            var updatedItem = GetItem(id);

            conn.Open();
            MySqlCommand cmd = new MySqlCommand("UPDATE item " +
                "SET " + "photo = @photo ," +
                "name = " + "@name ," +
                "category = " + "@category ," +
                "price = " + "@price , " +
                "description = " + "@description " +
                "WHERE id = @id", conn);

            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@name", item.Name);
            cmd.Parameters.AddWithValue("@category", item.Category);
            cmd.Parameters.AddWithValue("@price", item.Price);
            cmd.Parameters.AddWithValue("@description", item.Description);
            if (item.Photo == null)
            {
                cmd.Parameters.Add("@photo", MySqlDbType.Blob).Value = updatedItem.Photo;
            }
            else
            {
                cmd.Parameters.Add("@photo", MySqlDbType.Blob).Value = item.Photo;
            }

            var r = cmd.ExecuteNonQuery();

            if (r == 1)
                return true;
            else
                return false;
        }

        public bool CheckUser(string username, string password)
        {
            User user = null;

            using MySqlConnection conn = GetConnection();

            conn.Open();
            MySqlCommand cmd = new MySqlCommand("select * from user where user.username = '" + username + "' and user.password='" + password + "'", conn);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                string loggedUser = reader["Username"].ToString();
                string address = reader["Address"].ToString();
                string phoneNumber = reader["PhoneNumber"].ToString();
                user = new User() { Username = loggedUser, Address = address, PhoneNumber = phoneNumber };
            }

            if (user == null)
                return false;
            else
                return true;

        }

        public List<CardItem> GetUserBasket(string username)
        {
            List<CardItem> list = new List<CardItem>();

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("select * from item inner join userbasket on item.id = userbasket.item_id where userbasket.username = '" + username + "'", conn);

                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    list.Add(new CardItem()
                    {
                        Id = Int32.Parse(reader["card_id"].ToString()),
                        Item = new Item()
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Name = reader["Name"].ToString(),
                            Category = reader["Category"].ToString(),
                            Price = float.Parse(reader["Price"].ToString()),
                            Description = reader["Description"].ToString(),
                            Photo = (byte[])reader["photo"]
                        }
                    });
                }
            }

            return list;
        }

        public bool DeleteCartItem(int id)
        {
            using MySqlConnection conn = GetConnection();

            conn.Open();
            MySqlCommand cmd = new MySqlCommand("delete from userbasket where card_id=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);

            int result = cmd.ExecuteNonQuery();
            if (result == 1)
                return true;
            else
            {
                return false;
            }
        }

        public List<Order> GetAllOrders()
        {
            List<Order> orders = new List<Order>();


            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("select * from orders", conn);

                using var reader = cmd.ExecuteReader();
                Order order = null;
                while (reader.Read())
                {
                    order = new Order()
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        User = GetUserByName(reader["username"].ToString()),
                        OrderTime = DateTime.Parse(reader["order_time"].ToString()),
                        Price = float.Parse(reader["total_price"].ToString()),
                        Status = (OrderStatus)reader["order_status"],
                        PaymentType = (PaymentType)reader["payment_type"]
                    };
                    if (order != null)
                    {
                        order.Items = GetOrderItems(order.Id);
                        orders.Add(order);

                    }
                }

            }
            return orders;
        }

        public Order GetOrder(int orderId)
        {
            Order order = null;
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("select * from orders where id = @orderId", conn);
                cmd.Parameters.AddWithValue("@orderId", orderId);

                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    order = new Order()
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        User = GetUserByName(reader["username"].ToString()),
                        OrderTime = DateTime.Parse(reader["order_time"].ToString()),
                        Price = float.Parse(reader["total_price"].ToString()),
                        Status = (OrderStatus)reader["order_status"],
                        PaymentType = (PaymentType)reader["payment_type"]
                    };
                }
                if (order != null)
                {
                    order.Items = GetOrderItems(order.Id);
                }
            }
            return order;
        }

        public bool CreateOrder(Order order)
        {
            using MySqlConnection conn = GetConnection();

            conn.Open();
            MySqlCommand cmd = new MySqlCommand("insert into orders(username,order_time,total_price,order_status,payment_type) values(@username,@order_time,@total_price,@order_status,@payment_type)", conn);

            cmd.Parameters.AddWithValue("@username", order.User.Username);
            cmd.Parameters.AddWithValue("@order_time", order.OrderTime);
            cmd.Parameters.AddWithValue("@total_price", order.Price);
            cmd.Parameters.AddWithValue("@order_status", (int)order.Status);
            cmd.Parameters.AddWithValue("@payment_type", order.PaymentType);
            var res = cmd.ExecuteNonQuery();

            var userOrders = GetUserOrders(order.User.Username);
            var id = userOrders.Last().Id;

            for (int i = 0; i < order.Items.Count(); i++)
            {
                MySqlCommand ord = new MySqlCommand("insert into order_items values(@order_id,@item_id)", conn);

                ord.Parameters.AddWithValue("@order_id", id);
                ord.Parameters.AddWithValue("@item_id", order.Items[i].Id);
                var r = ord.ExecuteNonQuery();
            }

            if (res == 1)
                return true;
            else
                return false;
        }

        public bool ChangeOrderStatus(int orderId, int status)
        {
            using MySqlConnection conn = GetConnection();

            conn.Open();
            MySqlCommand cmd = new MySqlCommand("UPDATE orders SET order_status = @order_status WHERE id = @id", conn);

            cmd.Parameters.AddWithValue("@order_status", status);
            cmd.Parameters.AddWithValue("@id", orderId);

            var r = cmd.ExecuteNonQuery();

            if (r == 1)
                return true;
            else
                return false;
        }

        public List<Order> GetUserOrders(string username)
        {
            List<Order> orders = new List<Order>();
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("select * from orders where username = '" + username + "'", conn);

                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    int orderId = Convert.ToInt32(reader["Id"]);
                    orders.Add(new Order()
                    {
                        Id = orderId,
                        User = GetUserByName(reader["username"].ToString()),
                        OrderTime = DateTime.Parse(reader["order_time"].ToString()),
                        Price = float.Parse(reader["total_price"].ToString()),
                        Items = GetOrderItems(orderId),
                        Status = (OrderStatus)int.Parse(reader["order_Status"].ToString())
                    });
                }
            }
            return orders;
        }

        public List<Item> GetOrderItems(int orderId)
        {
            List<Item> list = new List<Item>();

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("select * from item inner join order_items on item.id = order_items.item_id where order_items.order_id = '" + orderId + "'", conn);

                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    list.Add(new Item()
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Name = reader["Name"].ToString(),
                        Category = reader["Category"].ToString(),
                        Price = float.Parse(reader["Price"].ToString()),
                        Description = reader["Description"].ToString()
                    });
                }
            }

            return list;
        }

        public User GetUserByName(string username)
        {
            User user = null;

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("select * from user where user.username = '" + username + "'", conn);

                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string loggedUser = reader["Username"].ToString();
                    string address = reader["Address"].ToString();
                    string phoneNumber = reader["PhoneNumber"].ToString();
                    UserRole role = (UserRole)reader["role"];

                    user = new User() { Username = loggedUser, Address = address, PhoneNumber = phoneNumber, Role = role };
                }
            }
            if (user != null)
            {
                user.Basket = GetUserBasket(username);
            }

            return user;
        }

        public void AddUser(User user)
        {
            using MySqlConnection conn = GetConnection();

            conn.Open();
            MySqlCommand cmd = new MySqlCommand("insert into user values(@username,@password,@address,@phoneNumber,@role)", conn);

            cmd.Parameters.AddWithValue("@username", user.Username);
            cmd.Parameters.AddWithValue("@password", user.Password);
            cmd.Parameters.AddWithValue("@address", user.Address);
            cmd.Parameters.AddWithValue("@phoneNumber", user.PhoneNumber);
            cmd.Parameters.AddWithValue("@role", user.Role);

            cmd.ExecuteNonQuery();

        }

        public void AddItem(Item item)
        {
            using MySqlConnection conn = GetConnection();

            conn.Open();
            MySqlCommand cmd = new MySqlCommand("insert into item(photo,name,category,price,description) values(@image,@name,@category,@price,@description)", conn);

            cmd.Parameters.AddWithValue("@name", item.Name);
            cmd.Parameters.AddWithValue("@category", item.Category);
            cmd.Parameters.AddWithValue("@price", item.Price);
            cmd.Parameters.AddWithValue("@description", item.Description);
            cmd.Parameters.Add("@image", MySqlDbType.Blob).Value = item.Photo;

            cmd.ExecuteNonQuery();

        }

        public bool DeleteItem(int id)
        {
            using MySqlConnection conn = GetConnection();

            conn.Open();
            MySqlCommand cmd = new MySqlCommand("delete from item where id=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);

            int result = cmd.ExecuteNonQuery();
            if (result == 1)
                return true;
            else
            {
                return false;
            }
        }

        public bool AddItemToUserBasket(int itemId, string username)
        {
            using MySqlConnection conn = GetConnection();

            conn.Open();
            MySqlCommand cmd = new MySqlCommand("insert into userbasket(username,item_id) values(@username,@itemid)", conn);

            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@itemid", itemId);

            var result = cmd.ExecuteNonQuery();

            if (result == 1)
            {
                return true;
            }
            else
                return false;
        }
    }
}
