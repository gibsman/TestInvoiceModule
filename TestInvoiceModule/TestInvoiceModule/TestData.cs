using NLog;
using System;
using System.Collections.Generic;

namespace TestInvoiceModule
{
    public interface ITestData
    {
        List<Order> GenerateTestOrders();

        List<Order> GenerateTestOrders(int orderListCount);
    }
    public class RandomTestData : ITestData
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private int orderId;
        private List<Client> testClients;
        private List<Product> testProducts;
        
        public RandomTestData()
        {
            InitializeClientAndProductTestData();
            orderId = 0;
        }

        public List<Order> GenerateTestOrders()
        {
            Random rand = new Random();
            return GenerateTestOrders(rand.Next(500));
        }

        public List<Order> GenerateTestOrders(int orderListCount)
        {
            if (orderListCount < 0)
            {
                throw new ArgumentOutOfRangeException("orderListCount", "Order list count is less than zero");
            }
            Random rand = new Random();
            List<Order> testOrders = new List<Order>();
            for (int i = 0; i < orderListCount; i++)
            {
                List<Product> shuffledProducts = new List<Product>(testProducts);
                ShuffleProductList(shuffledProducts, rand);
                int orderProductCount = rand.Next(1, testProducts.Count + 1);
                List<OrderProduct> orderProducts = new List<OrderProduct>();
                for (int j = 0; j < orderProductCount; j++)
                {
                    Product randomProduct = shuffledProducts[j];
                    orderProducts.Add(new OrderProduct(randomProduct, rand.Next(1, 31)));
                }
                Client randomClient = testClients[rand.Next(testClients.Count)];
                testOrders.Add(new Order(orderId, randomClient, DateTime.Now, orderProducts));
                logger.Debug("Random order № {0} generated", orderId);
                orderId++;
            }
            return testOrders;
        }

        private static void ShuffleProductList(List<Product> products, Random rand)
        {
            int n = products.Count;
            while (n > 1)
            {
                n--;
                int k = rand.Next(n + 1);
                Product temp = products[k];
                products[k] = products[n];
                products[n] = temp;
            }
        }

        private void InitializeClientAndProductTestData()
        {
            testClients = new List<Client>()
            {
                new Client("John Johnson", "johnson@gmail.com", "13 FeelGood st, Ban-Francisco", "123-456-7890"),
                new Client("Tom Tompson", "tompson@gmail.com", "10 Bright st, Berlin", "321-432-5278"),
                new Client("Ken Kenson", "kenson@gmail.com", "3 Snowy st, Paris", "222-632-1234"),
                new Client("Ben Benson", "benson@gmail.com", "65 Blunder st, Byzantium", "423-543-3142"),
                new Client("Jim Jimson", "jimson@gmail.com", "66 Satan st, Hell", "666-666-1313"),
                new Client("Dan Danson", "danson@gmail.com", "1 Best st, London", "412-675-4358"),
                new Client("Vin Vinson", "vinson@gmail.com", "13 Muscly st, Gymia", "435-765-4312"),
                new Client("Ron Ronson", "ronson@gmail.com", "11 Stuffy st, Liverpool", "542-888-453"),
                new Client("One Oneson", "oneson@gmail.com", "1 First st, Unitia", "111-111-1111"),
                new Client("Stan Stanson", "stanson@gmail.com", "31 Rem st, Lezaria", "534-123-6754")
            };
            testProducts = new List<Product>()
            {                
                new Product("Hammer", 5),
                new Product("Pliers", 6),
                new Product("Wire cutter", 4),
                new Product("Screwdriver", 2),
                new Product("Saw", 9),
                new Product("Scissors", 2),
                new Product("Utility knife", 12),
                new Product("Drill", 30),
                new Product("Flashlight", 15),
                new Product("Electrical tape", 5)
            };
        }
    }
}
