﻿using System;
using System.IO;
using Org.Unidal.Cat;
using System.Threading;
using CatClientTest.PerformanceTest;
using System.Collections.Generic;
using Org.Unidal.Cat.Message;

namespace CatClientTest
{
    public class Program
    {
        static void Main()
        {
            try
            {
                SimpleTest();
            }
            finally
            {
                if (null != Cat.lastException)
                {
                    Console.WriteLine("Cat.lastException:\n" + Cat.lastException);
                }
                Console.WriteLine("Test ends successfully. Press any key to continue");
                Console.Read();
            }
        }

        private static void SimpleTest()
        {
            
            Console.WriteLine("Start: " + DateTime.Now);
            ITransaction newOrderTransaction = null;
            ITransaction paymentTransaction = null;
            try
            {
                newOrderTransaction = Cat.NewTransaction("SimpleTest", "NewTrainOrder");
                newOrderTransaction.AddData("I am a detailed message");
                newOrderTransaction.AddData("another message");
                Cat.LogEvent("TrainNo", "123456");
                Cat.LogError("MyException", new Exception("My Exception"));

                try
                {
                    paymentTransaction = Cat.NewTransaction("NewPayment", "PaymentDetail");
                    paymentTransaction.Status = CatConstants.SUCCESS;
                }
                catch (Exception ex)
                {
                    paymentTransaction.SetStatus(ex);
                }
                finally
                {
                    paymentTransaction.Complete();
                }

                newOrderTransaction.Status = CatConstants.SUCCESS;
            }
            catch (Exception ex)
            {
                newOrderTransaction.SetStatus(ex);
            }
            finally
            {
                newOrderTransaction.Complete();
                Console.WriteLine("End: " + DateTime.Now);
            }
        }
    }
}