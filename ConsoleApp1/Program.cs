using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp1
{
    class Program
    {
        public static object NewtonSoft { get; private set; }

        static void Main(string[] args)
        {
            //Console.WriteLine("Hello World!");
            // BrokeLoop2();
            //Question3();
            Question4();

            Console.ReadLine();
        }

        static void BrokeLoop2()
        {
            var i = 0;
            while (i++ < 6)
            {
                if (i != 3)
                {
                    Console.WriteLine(i);
                }
            }
        }


        static void Question4()
        {
            var stock_prices = new List<KeyValuePair<KeyValuePair<string, int>, KeyValuePair<string, int>>>
            {
                new KeyValuePair<KeyValuePair<string, int>, KeyValuePair<string, int>>(
                    new KeyValuePair<string, int>("Day",0),
                    new KeyValuePair<string, int>("Price", 5)),
                new KeyValuePair<KeyValuePair<string, int>, KeyValuePair<string, int>>(
                    new KeyValuePair<string, int>("Day",1),
                    new KeyValuePair<string, int>("Price", 9)),
                new KeyValuePair<KeyValuePair<string, int>, KeyValuePair<string, int>>(
                    new KeyValuePair<string, int>("Day",2),
                    new KeyValuePair<string, int>("Price", 3)),
                new KeyValuePair<KeyValuePair<string, int>, KeyValuePair<string, int>>(
                    new KeyValuePair<string, int>("Day",3),
                    new KeyValuePair<string, int>("Price", 5)),
                new KeyValuePair<KeyValuePair<string, int>, KeyValuePair<string, int>>(
                    new KeyValuePair<string, int>("Day",4),
                    new KeyValuePair<string, int>("Price", 8)),
                new KeyValuePair<KeyValuePair<string, int>, KeyValuePair<string, int>>(
                    new KeyValuePair<string, int>("Day",5),
                    new KeyValuePair<string, int>("Price", 7)),
                new KeyValuePair<KeyValuePair<string, int>, KeyValuePair<string, int>>(
                    new KeyValuePair<string, int>("Day",8),
                    new KeyValuePair<string, int>("Price", 9)),
            };

            var x = BuySellOptimizer(stock_prices);
            Console.WriteLine(x);
        }

        class BuySellPairing
        {
            public Stock Buy { get; set; }
            public Stock Sell {get;set;}
            public int Profit
            {
                get
                {
                    if (Sell.Day > Buy.Day)
                        return Sell.Price - Buy.Price;
                    else
                        return 0;
                }
            }
        }    
        class Stock
        {
            public Stock(KeyValuePair<KeyValuePair<string, int>, KeyValuePair<string, int>> keyValuePair)
            {
                Day = keyValuePair.Key.Value;
                Price = keyValuePair.Value.Value;
            }
            public int Day { get; set; }
            public int Price { get; set; }
        }

        //assumed we have a finite list and that it's not exponentially large
        static BuySellPairing BuySellOptimizer(List<KeyValuePair<KeyValuePair<string, int>, KeyValuePair<string, int>>> stockPrices)
        {
            var stockCnt = stockPrices.Count;
            var buySell = new BuySellPairing();
            var result = new List<BuySellPairing>();

            var orderedByDays = stockPrices.OrderBy(q => q.Key.Value);
            // Prices must be given for at least two days 
            if (stockCnt == 1)
                throw new Exception("Invalid data, need multiple days");

            var maxPrice = orderedByDays.Max(q => q.Value.Value);
            var minPrice = orderedByDays.Min(q => q.Value.Value);
            var maxDay = orderedByDays.Max(q => q.Key.Value);
            var minDay = orderedByDays.Min(q => q.Key.Value);
            var duplicateDays = orderedByDays.GroupBy(q => q.Key.Value).Where(q => q.Count() > 1);

            if (duplicateDays.Count() > 0)
                throw new Exception("Invalid data, can not submit prices for multiple days");
            if (minPrice < 0 || maxPrice > 32000)
                throw new Exception("Invalid data, price must be betwee 0 and 32000");
            if (minDay < 0 || maxDay > 32000)
                throw new Exception("Invalid data, price must be betwee 0 and 32000");

            int count = 0;
            int x = 0;
            while (x < stockCnt - 2)
            {
                var y = x;
                // Find the minimum value relative to the index    
                //prime the day we buy, have to buy a stock so let's assume we buy day 1
                buySell.Buy = new Stock(orderedByDays.ElementAt(x));

                while (y < stockCnt - 1)
                {
                    Console.WriteLine($"{orderedByDays.ElementAt(y + 1).Value.Value} <= {orderedByDays.ElementAt(y).Value.Value}");
                    if (orderedByDays.ElementAt(y + 1).Value.Value <= orderedByDays.ElementAt(y).Value.Value
                        && buySell.Buy.Price > orderedByDays.ElementAt(y + 1).Value.Value)
                        buySell.Buy = new Stock(orderedByDays.ElementAt(y + 1));

                    y++;
                }

                Console.WriteLine($"Buy { JsonConvert.SerializeObject(buySell.Buy)}");

                //can't buy and sell on the same day
                var z = x + 1;
                //Find the maximum value relative to the index
                //prime the day we buy, have to sell a stock so let's assume we sell day 2
                buySell.Sell = new Stock(orderedByDays.ElementAt(z));
                //can't buy and sell on same day               
                while (z < stockCnt - 2) {
                    if (orderedByDays.ElementAt(z).Value.Value >= orderedByDays.ElementAt(z - 1).Value.Value
                        && buySell.Sell.Price < orderedByDays.ElementAt(z).Value.Value)
                        buySell.Sell = new Stock(orderedByDays.ElementAt(z));
                    z++;
                }

                // Save the sell index                
                Console.WriteLine($"Sell { JsonConvert.SerializeObject(buySell.Sell)}");

                result.Add(buySell);

                // Increment number of buy/sell 
                count++;
                x++;
            }

            if (count == 0)             
                throw new Exception("Unable to turn a profile, get out of the market.");
            else            
                return result.OrderBy(q=>q.Profit).Last();           
        }

        static void Question3()
        {
            //Initializing initial dataset
            //int[] intList = new int[] { 19, 6, 2, 3, 5, 5,5,5,5,5,5,5,5,5,5,4, 1, 4, 4, 3, 3, 3,6,6,6,6,6,7,8,9,10,11,12,13,14,15,16,16,17,18,19,19,19,19,19,19,19,19,19, 5,5,5,5,5,5,5,5,5,5,5,5};
            //int[] intList = new int[] { 6, 2, 3, 4, 1, 4, 4 };
            //int[] intList = new int[] {1,2,3,1,2,3,4,5,6,7,8 };
            int[] intList = new int[] { 9, 8, 7, 6, 5, 4, 3, 2, 1 };

            //determines the number elements in the dataset
            int arrSize = intList.Length;
            int result = find(intList, arrSize);

            //outou
            Console.WriteLine(result);
        }

        //find most frequent number that is greater than 1 and less than half the number of elements in the list 
        static int find(int[] arr, int size)
        {
            //pointers used to navigate the arr[]
            int floor = 1;
            int ceiling = size - 1;

            int lower_floor, lower_ceiling, upper_floor, upper_ceiling, items_in_lower_range, distinct_possible_integers_in_lower_range;


            while (floor < ceiling)
            {
                Console.WriteLine(string.Format($"floor {floor} <  ceiling {ceiling}"));

                //get midpoint of the current pointers
                int midpoint = floor + ((ceiling - floor) / 2);                

                lower_floor = floor;
                lower_ceiling = midpoint;
                upper_floor = midpoint + 1;
                upper_ceiling = ceiling;

                Console.WriteLine(string.Format($"LF: {lower_floor} LC: {lower_ceiling} UF: {upper_floor} UC: {upper_ceiling}"));
                items_in_lower_range = 0;

                for (int i = 0; i < size; i++)
                {
                    Console.WriteLine(string.Format($"arr[i] {arr[i]} >= LF: {lower_floor} && arr[i] {arr[i]} <= LC: {lower_ceiling}"));
                    //determine if array element falls within the predetermined range
                    if (arr[i] >= lower_floor && arr[i] <= lower_ceiling)
                    {
                        items_in_lower_range++;
                        Console.WriteLine($"Items++ {items_in_lower_range}");
                    }
                }

                //determine total possible distinct elements in section
                distinct_possible_integers_in_lower_range = lower_ceiling - lower_floor + 1;
                Console.WriteLine($"distinct ints in lower range { distinct_possible_integers_in_lower_range}");

                if (items_in_lower_range > distinct_possible_integers_in_lower_range)
                {
                    Console.WriteLine("ITems in lower range > distinct");
                    floor = lower_floor;
                    ceiling = lower_ceiling;                    
                }
                else
                {
                    Console.WriteLine("Items < distinct");
                    floor = upper_floor;
                    ceiling = upper_ceiling;
                }
                Console.WriteLine($"Floor = {floor} Ceiling = {ceiling}");

            }

            return floor;
        }
    }
}




////#include <iostream>
////#include <array>

////using namespace std;

////// function declaration:
////int find(int arr[], int size);

////int main()
////{
////    //Initializing initial dataset
////    int intList[7] = { 6, 2, 3, 4, 1, 4, 4 };

////    //determines the number elements in the dataset
////    int arrSize = sizeof(intList) / sizeof(*intList);
////    //main processing
////    int result = find(intList, arrSize);

////    //output result
////    cout << result;
////}

////// do some whacky logic on an array list
////int find(int arr[], int size)
////{
////    int floor = 1;
////    int ceiling = size - 1;

////    int lower_floor, lower_ceiling, upper_floor, upper_ceiling, items_in_lower_range, distinct_possible_integers_in_lower_range;

////    while (floor < ceiling)
////    {
////        //get new midpoint
////        int midpoint = floor + ((ceiling - floor) / 2);

////        //reset the 2 quadrants that we want to inspect each time, always splitting the available list in half
////        lower_floor = floor;
////        lower_ceiling = midpoint;
////        upper_floor = midpoint + 1;
////        upper_ceiling = ceiling;

////        items_in_lower_range = 0;

////        //loop through all values of the array on each pass
////        for (int i = 0; i < size; i++)
////        {
////            //if array item is between the lower floower and lower ceiling, increment counter
////            if (arr[i] >= lower_floor && arr[i] <= lower_ceiling)
////            {
////                items_in_lower_range++;
////            }
////        }

////        //determine maximum number of values that are allowed between lower ceiling and lower floor + 1
////        distinct_possible_integers_in_lower_range = lower_ceiling - lower_floor + 1;

////        //if the number of found items is greater than the maximum numbers allowed, move the floor and celing to the lower section of the list
////        if (items_in_lower_range > distinct_possible_integers_in_lower_range)
////        {
////            floor = lower_floor;
////            ceiling = lower_ceiling;
////        }
////        //if the number of found items in the list is less than the maximum number of values allowed, move the floor and ceiling to the upper section of the list
////        else
////        {
////            floor = upper_floor;
////            ceiling = upper_ceiling;
////        }

////    }

////    return floor;
////}
