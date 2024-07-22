using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

// Placing the Purchaser class in the CompleteProject namespace allows it to interact with ScoreManager, 
// one of the existing Survival Shooter scripts.
namespace CompleteProject
{
    // Deriving the Purchaser class from IStoreListener enables it to receive messages from Unity Purchasing.
    public class Purchaser : MonoBehaviour, IStoreListener
    {
        private static IStoreController m_StoreController;          // The Unity Purchasing system.
        private static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.

        // Product identifiers for all products capable of being purchased: 
        // "convenience" general identifiers for use with Purchasing, and their store-specific identifier 
        // counterparts for use with and outside of Unity Purchasing. Define store-specific identifiers 
        // also on each platform's publisher dashboard (iTunes Connect, Google Play Developer Console, etc.)

        // General product identifiers for the consumable, non-consumable, and subscription products.
        // Use these handles in the code to reference which product to purchase. Also use these values 
        // when defining the Product Identifiers on the store. Except, for illustration purposes, the 
        // kProductIDSubscription - it has custom Apple and Google identifiers. We declare their store-
        // specific mapping to Unity Purchasing's AddProduct, below.
        public static string kProductIDConsumable = "consumable";
        //public static string kProductIDNonConsumable = "nonconsumable";
        public static string kProductIDSubscription = "subscription"; // nonconsumable

        //public static string kProductIDSuperPion = "super_palet_embrase";
        //public static string kProductIDIllimitedTickets = "tickets_online_infinis";

        // NOMS PAR Défaut, ceux de googlePlay
        // Ces variables servent seulement pour identifier typeOffre, et pas utilisés comme id dans les requêtes d'achat
        public static string kProductIDMoreGemmes_90 = "buy_more_monney";
        public static string kProductIDMoreGemmes_500 = "buy_more_monney_2900";
        public static string kProductIDMoreGemmes_1200 = "buy_more_monney_1200";
        public static string kProductIDOffer_debutant = "pack_offer_debutant";

        //private static string currentIDMoreMonney;

        private Dictionary<string, string> listKeyProduct_google = new Dictionary<string, string>()
        {
            {kProductIDMoreGemmes_90, "buy_more_monney"},
            {kProductIDMoreGemmes_500, "buy_more_monney_2900"},
            {kProductIDMoreGemmes_1200, "buy_more_monney_1200"},
            {kProductIDOffer_debutant, "pack_offer_debutant"},
        };

        private Dictionary<string, string> listKeyProduct_ios = new Dictionary<string, string>()
        {
            {kProductIDMoreGemmes_90, "90Gem"},
            {kProductIDMoreGemmes_500, "500Gem"},
            {kProductIDMoreGemmes_1200, "1200Gem"},
            {kProductIDOffer_debutant, "packOfferDebutant"},
        };

        private string getKeyStringByPlatform(string key)
        {
            if (Application.isEditor)
            {
                return key;
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                return listKeyProduct_ios[key];
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
                return listKeyProduct_google[key];
            }
            return key;
        }

        // Apple App Store-specific product identifier for the subscription product.
        //private static string kProductNameAppleSubscription = "com.DefaultCompany.Yoyow";

        // Google Play Store-specific product identifier subscription product.
        //private static string kProductNameGooglePlaySubscription = "com.DefaultCompany.Yoyow";

        void Start()
        {
            // If we haven't set up the Unity Purchasing reference
            if (m_StoreController == null)
            {
                // Begin to configure our connection to Purchasing
                InitializePurchasing();
            }
        }

        public void InitializePurchasing()
        {
            if (IsInitialized())
            {
                return;
            }

            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            builder.AddProduct(kProductIDMoreGemmes_90, ProductType.Consumable, new IDs
    {
        { listKeyProduct_google[kProductIDMoreGemmes_90], GooglePlay.Name },
        { listKeyProduct_ios[kProductIDMoreGemmes_90], AppleAppStore.Name }
    });
            builder.AddProduct(kProductIDMoreGemmes_500, ProductType.Consumable, new IDs
    {
        { listKeyProduct_google[kProductIDMoreGemmes_500], GooglePlay.Name },
        { listKeyProduct_ios[kProductIDMoreGemmes_500], AppleAppStore.Name }
    });
            builder.AddProduct(kProductIDMoreGemmes_1200, ProductType.Consumable, new IDs
    {
        { listKeyProduct_google[kProductIDMoreGemmes_1200], GooglePlay.Name },
        { listKeyProduct_ios[kProductIDMoreGemmes_1200], AppleAppStore.Name }
    });
            builder.AddProduct(kProductIDOffer_debutant, ProductType.NonConsumable, new IDs
    {
        { listKeyProduct_google[kProductIDOffer_debutant], GooglePlay.Name },
        { listKeyProduct_ios[kProductIDOffer_debutant], AppleAppStore.Name }
    });

            UnityPurchasing.Initialize(this, builder);
        }



        private bool IsInitialized()
        {
            // Only say we are initialized if both the Purchasing references are set.
            return m_StoreController != null && m_StoreExtensionProvider != null;
        }

        public void BuyPackOffer(typePackOffer typePack)
        {
            string id = "";
            if (typePack == typePackOffer.pack_debutant)
            {
                id = getKeyStringByPlatform(kProductIDOffer_debutant);
            }

            if(id == "")
            {
                packOffer.currentInstanceTryBuy.failedToPurchase(true);
            }
            else
            {
                BuyProductID(id);
            }
        }


        public void BuyMoreMonney(offerGemmesType offerType)
        {
            string id = offerType == offerGemmesType.offer_90 ? getKeyStringByPlatform(kProductIDMoreGemmes_90)
                : offerType == offerGemmesType.offer_500 ? getKeyStringByPlatform(kProductIDMoreGemmes_500)
                : getKeyStringByPlatform(kProductIDMoreGemmes_1200);
            BuyProductID(id);
        }


        void BuyProductID(string productId)
        {
            // If Purchasing has been initialized ...
            if (IsInitialized())
            {
                // ... look up the Product reference with the general product identifier and the Purchasing 
                // system's products collection.
                Product product = m_StoreController.products.WithID(productId);

                // If the look up found a product for this device's store and that product is ready to be sold ... 
                if (product != null && product.availableToPurchase)
                {
                    Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                    // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
                    // asynchronously.
                    m_StoreController.InitiatePurchase(product);
                }
                // Otherwise ...
                else
                {
                    if (productId == getKeyStringByPlatform(kProductIDMoreGemmes_90)
                        || productId == getKeyStringByPlatform(kProductIDMoreGemmes_500)
                        || productId == getKeyStringByPlatform(kProductIDMoreGemmes_1200))
                    {
                        gemmesManager.moreGemmesPurchaseResult(false);
                    }
                    else if (productId == getKeyStringByPlatform(kProductIDOffer_debutant))
                    {
                        packOffer.currentInstanceTryBuy.failedToPurchase(true);
                    }

                    // ... report the product look-up failure situation  
                    Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                }
            }
            // Otherwise ...
            else
            {
                if (productId == getKeyStringByPlatform(kProductIDMoreGemmes_90)
                    || productId == getKeyStringByPlatform(kProductIDMoreGemmes_500)
                    || productId == getKeyStringByPlatform(kProductIDMoreGemmes_1200))
                {
                    gemmesManager.moreGemmesPurchaseResult(false);
                }
                else if (productId == getKeyStringByPlatform(kProductIDOffer_debutant))
                {
                    packOffer.currentInstanceTryBuy.failedToPurchase(true);
                }
                // more monney failed

                // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
                // retrying initiailization.
                Debug.Log("BuyProductID FAIL. Not initialized.");
            }
        }

        // ON NE FAIT RIEN, UnityCloud save player et restore lui même ses achats
        // Restore purchases previously made by this customer. Some platforms automatically restore purchases, like Google. 
        // Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.
        public void RestorePurchases()
        {
            // If Purchasing has not yet been set up ...
            if (!IsInitialized())
            {
                // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
                Debug.Log("RestorePurchases FAIL. Not initialized.");
                return;
            }

            // If we are running on an Apple device ... 
            if (Application.platform == RuntimePlatform.IPhonePlayer ||
                Application.platform == RuntimePlatform.OSXPlayer)
            {
                // ... begin restoring purchases
                Debug.Log("RestorePurchases started ...");

                // Fetch the Apple store-specific subsystem.
                var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
                // Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
                // the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
                apple.RestoreTransactions((result) => {
                    // The first phase of restoration. If no more responses are received on ProcessPurchase then 
                    // no purchases are available to be restored.
                    Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
                });
            }
            // Otherwise ...
            else
            {
                // We are not running on an Apple device. No work is necessary to restore purchases.
                Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
            }
        }


        //  
        // --- IStoreListener
        //

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            // Purchasing has succeeded initializing. Collect our Purchasing references.
            Debug.Log("OnInitialized: PASS");

            // Overall Purchasing system, configured with products for this application.
            m_StoreController = controller;
            // Store specific subsystem, for accessing device-specific store features.
            m_StoreExtensionProvider = extensions;
        }


        public void OnInitializeFailed(InitializationFailureReason error)
        {
            // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
            Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
        }

        // achat réussit
        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            if (String.Equals(args.purchasedProduct.definition.id
                , m_StoreController.products.WithID(getKeyStringByPlatform(kProductIDOffer_debutant)).definition.id, StringComparison.Ordinal))
            {
                packOffer.currentInstanceTryBuy.succedPurchase();
            }
            
            else if (
                String.Equals(args.purchasedProduct.definition.id
                , m_StoreController.products.WithID(getKeyStringByPlatform(kProductIDMoreGemmes_90)).definition.id, StringComparison.Ordinal)
                || String.Equals(args.purchasedProduct.definition.id
                , m_StoreController.products.WithID(getKeyStringByPlatform(kProductIDMoreGemmes_500)).definition.id, StringComparison.Ordinal)
                || String.Equals(args.purchasedProduct.definition.id
                , m_StoreController.products.WithID(getKeyStringByPlatform(kProductIDMoreGemmes_1200)).definition.id, StringComparison.Ordinal))
            {
                gemmesManager.moreGemmesPurchaseResult(true);
            }
            else
            {
                Debug.Log(string.Format("ProcessPurchase: FAIL. Unrecognized product: '{0}'", args.purchasedProduct.definition.id));
            }

            // Return a flag indicating whether this product has completely been received, or if the application needs 
            // to be reminded of this purchase at next app launch. Use PurchaseProcessingResult.Pending when still 
            // saving purchased products to the cloud, and when that save is delayed. 
            return PurchaseProcessingResult.Complete;
        }

        // achat fail
        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            if (product.definition.id == m_StoreController.products.WithID(getKeyStringByPlatform(kProductIDMoreGemmes_90)).definition.id
                || product.definition.id == m_StoreController.products.WithID(getKeyStringByPlatform(kProductIDMoreGemmes_500)).definition.id
                || product.definition.id == m_StoreController.products.WithID(getKeyStringByPlatform(kProductIDMoreGemmes_1200)).definition.id)
            {
                gemmesManager.moreGemmesPurchaseResult(false);
            }
            else if (product.definition.id == m_StoreController.products.WithID(getKeyStringByPlatform(kProductIDOffer_debutant)).definition.id)
                packOffer.currentInstanceTryBuy.failedToPurchase(false);
            // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
            // this reason with the user to guide their troubleshooting actions.
            Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            //throw new NotImplementedException();
        }
    }
}