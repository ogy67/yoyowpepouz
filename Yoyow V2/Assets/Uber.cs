using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Uber : MonoBehaviour
{
    public int nbBeer = 0, nbChips = 0;
    public bool commandPizza = false, commandBeer = false, commandCoca = false, commandChips = false;
    public bool BTN_searchCommand = false;
    public float priceToReach = 10f;

    private void Update()
    {
        if (BTN_searchCommand)
        {
            BTN_searchCommand = false;
            searchCommand();
        }
    }

    private void searchCommand()
    {
        List<Dictionary<string, float>> articlesAsked = new List<Dictionary<string, float>>();
        if (commandPizza)
            articlesAsked.Add(pricePizza);
        if (commandBeer)
            for (int i = 0; i < nbBeer; i++)
                articlesAsked.Add(priceBeer);
        if (commandCoca)
            articlesAsked.Add(priceCoca);
        if (commandChips)
            for (int i = 0; i < nbChips; i++)
                articlesAsked.Add(priceChips);

        searchCommandReccursiv(articlesAsked, new List<string>(), 0f);
    }

    private void searchCommandReccursiv(List<Dictionary<string, float>> articlesAsked,
        List<string> currentCommand, float currentPrice)
    {
        if (currentPrice > priceToReach)
            return;

        if (currentPrice == priceToReach)
        {
            Debug.Log("<color=blue>COMBI FOUND</color>");
            foreach (string s in currentCommand)
                Debug.Log(s);
        }
        else if (articlesAsked.Count != 0)
        {
            List<Dictionary<string, float>> articlesAsked_2
                = new List<Dictionary<string, float>>(articlesAsked);
            articlesAsked_2.RemoveAt(0);


            foreach (KeyValuePair<string, float> kV in articlesAsked[0])
            {
                List<string> currentCommand_2 = new List<string>(currentCommand);
                currentCommand_2.Add(kV.Key);
                float currentPrice_2 = currentPrice + kV.Value;

                searchCommandReccursiv(articlesAsked_2, currentCommand_2, currentPrice_2);
            }
        }
    }

    Dictionary<string, float> pricePizza = new Dictionary<string, float>()
    {
        {"Pizza au chevre", 3.93f },
         {"Pizza royale", 3.56f },
          {"Pizza tono", 3.38f },
           {"Pizza quatro fromagi", 3.55f },
            {"Pizza mozarella", 3.36f },
            {"Pizza flammem", 3.25f },
    };
    Dictionary<string, float> priceBeer = new Dictionary<string, float>()
    {
        {"Biere despe normal", 3.34f },
         {"Biere despe rouge", 3.16f },
         {"Biere estribos", 2.35f },
          {"Biere goudale", 3.18f },
          {"Biere leffe 0.5L", 2.67f },
          {"Biere kronembourg 7.2", 1.7f },
    };
    Dictionary<string, float> priceCoca = new Dictionary<string, float>()
    {
        {"Coca 1L", 1.91f },
         {"Coca 1.25L", 2.11f },
          {"Coca 6 canettes", 3.98f },
          {"OASIS 1L", 2.35f },
          {"Energy crazy 0.5L", 1.39f },
          {"Monster normal 0.5L", 1.62f },
    };
    Dictionary<string, float> priceChips = new Dictionary<string, float>()
    {
        {"Chips 3D", 1.23f },
         {"Chips Lays Barbecue", 1.26f },
          {"Chips Pringles Nature", 2.44f },
          {"Chips Doritos fromage", 2.18f },
          {"Chips Crème oignon", 1.56f },
          {"Chips Paprika", 1.67f },
    };
}
