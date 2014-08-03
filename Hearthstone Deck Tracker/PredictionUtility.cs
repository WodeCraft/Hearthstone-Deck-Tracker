using Hearthstone_Deck_Tracker.Hearthstone;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Hearthstone_Deck_Tracker
{
    public class PredictionUtility
    {
        //public delegate void MyDelegateEventHandler(object sender, PredictionEventArgs e);
        public event EventHandler<PredictionEventArgs> PopularDecksChanged;

        private bool noDeckFound = false;
        private string _deckPath = string.Empty;
        private Decks _popularDecks;

        public string PlayingClass { get; set; }
        public ObservableCollection<Deck> PotentialDecks;

        public PredictionUtility()
        {
            try
            {
                _deckPath = Config.Instance.AppDataPath + @"\PopularDecks.xml";

                if (!File.Exists(_deckPath))
                {
                    CreateDummyDeckList();
                }
                _popularDecks = XmlManager<Decks>.Load(_deckPath);
                PotentialDecks = new ObservableCollection<Deck>();
            }
            catch (Exception)
            {
                _popularDecks = new Decks();
                _popularDecks.DecksList = new ObservableCollection<Deck>();
                PotentialDecks = new ObservableCollection<Deck>();
            }

        }


        /// <summary>
        /// Method for guessing what deck the opponent is playing.
        /// The method is using a list of know decks <c>_popularDecks</c> as well as a list of potential decks <c>_potentialDecks</c>.
        /// <c>_potentialDecks</c> will initially be filled with all know decks filtered on the opponent class.
        /// Every time the method is called it will check the played card <paramref name="cardPlayed"/> against all the decks in the <c>_potentialDecks</c>.
        /// If a match is not found that deck will then be removed from the list.
        /// The methos will also validate that the number of times the card is played matches the amount in the deck. 
        /// Ex. if the card is only in the deck once, but this is the second time the opponent is playing that card, then the current deck will be removed from the list.
        /// </summary>
        /// <param name="cardPlayed">The latest card played</param>
        public void GuessDeck(Card cardPlayed)
        {
            // TODO Should be able to return the potential decks for the UI to do with as it pleases
            bool deckChanges = false;

            // If _popularDecks is empty, just return immediately
            if (_popularDecks.DecksList.Count == 0)
            {
                return;
            }

            // If _potentialDecks has an empty decklist, then select all known decks for the class being played.
            if (PotentialDecks.Count == 0 && !noDeckFound)
            {
                IEnumerable<Deck> ieDeck = _popularDecks.DecksList.Where(d => d.Class.Equals(PlayingClass));
                //PotentialDecks = new ObservableCollection<Deck>(ieDeck);
                foreach (Deck deck in ieDeck)
                {
                    PotentialDecks.Add(deck);
                    deckChanges = true;
                }
            }

            Logger.WriteLine(string.Format("Number of potential decks before prediction: {0}", PotentialDecks.Count));
            List<Deck> noMatches = new List<Deck>();
            foreach (var deck in PotentialDecks)
            {
                Card card = deck.Cards.FirstOrDefault(c => c.Name.Equals(cardPlayed.Name, StringComparison.InvariantCultureIgnoreCase));
                if (card == null || cardPlayed.Count > card.Count)
                {
                    // The played card has been used more times than it is possible with the current deck.
                    // Remove this deck from the potential decks
                    noMatches.Add(deck);
                }
            }
            // Remove all decks that had no matches
            foreach (var deck in noMatches)
            {
                deckChanges = true;
                PotentialDecks.Remove(deck);
                if (PotentialDecks.Count == 0)
                {
                    noDeckFound = true;
                }
            }

            Logger.WriteLine(string.Format("Number of potential decks after last prediction: {0}", PotentialDecks.Count));
            if (deckChanges)
            {
                if (PopularDecksChanged != null)
                {
                    PopularDecksChanged(this, new PredictionEventArgs(PotentialDecks.ToList<Deck>()));
                }
            }
        }

        /// <summary>
        /// Method for resetting the properties to be ready for a new prediction
        /// </summary>
        public void ResetPredictions()
        {
            noDeckFound = false;
            PlayingClass = string.Empty;
            PotentialDecks.Clear();

            try
            {
                _popularDecks = XmlManager<Decks>.Load(_deckPath);
            }
            catch (Exception)
            {
                _popularDecks = new Decks();
                _popularDecks.DecksList = new ObservableCollection<Deck>();
            }
        }

        /// <summary>
        /// This is only for test!!
        /// 
        /// Method for creating a default deck list of potential decks.
        /// The list will only include all Innkeeper Expert decks.
        /// </summary>
        private void CreateDummyDeckList()
        {
            try
            {
                List<Card> cards = Game.GetActualCards();

                Decks decks = new Decks();
                decks.DecksList = new ObservableCollection<Deck>();

                Deck deck = new Deck();
                deck.Class = "Paladin";
                deck.Name = "Innkeeper Paladin - Expert";

                Card card = Game.GetCardFromName("Argent Commander");
                card.Count = 2;
                deck.Cards.Add(card);

                card = Game.GetCardFromName("Argent Squire");
                card.Count = 2;
                deck.Cards.Add(card);

                card = Game.GetCardFromName("Blessing of Kings");
                card.Count = 2;
                deck.Cards.Add(card);

                card = Game.GetCardFromName("Blood Knight");
                card.Count = 2;
                deck.Cards.Add(card);

                card = Game.GetCardFromName("Consecration");
                card.Count = 2;
                deck.Cards.Add(card);

                card = Game.GetCardFromName("Equality");
                card.Count = 2;
                deck.Cards.Add(card);

                card = Game.GetCardFromName("Eye for an Eye");
                card.Count = 2;
                deck.Cards.Add(card);

                card = Game.GetCardFromName("Noble Sacrifice");
                card.Count = 2;
                deck.Cards.Add(card);

                card = Game.GetCardFromName("Redemption");
                card.Count = 2;
                deck.Cards.Add(card);

                card = Game.GetCardFromName("Repentance");
                card.Count = 2;
                deck.Cards.Add(card);

                card = Game.GetCardFromName("Scarlet Crusader");
                card.Count = 2;
                deck.Cards.Add(card);

                card = Game.GetCardFromName("Secretkeeper");
                card.Count = 2;
                deck.Cards.Add(card);

                card = Game.GetCardFromName("Silvermoon Guardian");
                card.Count = 2;
                deck.Cards.Add(card);

                card = Game.GetCardFromName("Spellbreaker");
                card.Count = 2;
                deck.Cards.Add(card);

                card = Game.GetCardFromName("Sunwalker");
                card.Count = 2;
                deck.Cards.Add(card);

                decks.DecksList.Add(deck);


                deck = new Deck();
                deck.Class = "Paladin";
                deck.Name = "Innkeeper Paladin - Basic";

                card = Game.GetCardFromName("Blessing of Might");
                card.Count = 2;
                deck.Cards.Add(card);

                card = Game.GetCardFromName("Elven Archer");
                card.Count = 1;
                deck.Cards.Add(card);

                card = Game.GetCardFromName("Gnomish Inventor");
                card.Count = 2;
                deck.Cards.Add(card);

                card = Game.GetCardFromName("Goldshire Footman");
                card.Count = 2;
                deck.Cards.Add(card);

                card = Game.GetCardFromName("Hammer of Wrath");
                card.Count = 2;
                deck.Cards.Add(card);

                card = Game.GetCardFromName("Hand of Protection");
                card.Count = 2;
                deck.Cards.Add(card);

                card = Game.GetCardFromName("Holy Light");
                card.Count = 2;
                deck.Cards.Add(card);

                card = Game.GetCardFromName("Ironforge Rifleman");
                card.Count = 2;
                deck.Cards.Add(card);

                card = Game.GetCardFromName("Light's Justice");
                card.Count = 2;
                deck.Cards.Add(card);

                card = Game.GetCardFromName("Lord of the Arena");
                card.Count = 2;
                deck.Cards.Add(card);

                card = Game.GetCardFromName("Novice Engineer");
                card.Count = 2;
                deck.Cards.Add(card);

                card = Game.GetCardFromName("Raid Leader");
                card.Count = 2;
                deck.Cards.Add(card);

                card = Game.GetCardFromName("Stormpike Commando");
                card.Count = 2;
                deck.Cards.Add(card);

                card = Game.GetCardFromName("Stormwind Champion");
                card.Count = 2;
                deck.Cards.Add(card);

                card = Game.GetCardFromName("Stormwind Knight");
                card.Count = 2;
                deck.Cards.Add(card);

                decks.DecksList.Add(deck);
                XmlManager<Decks>.Save(_deckPath, decks);
            }
            catch (Exception ex)
            {
                Logger.WriteLine(string.Format("Error occured during building of decklist: {0}", ex.Message));
            }

        }

    }

    public class PredictionEventArgs : EventArgs
    {
        public List<Deck> PotentialDecks { get; private set; }
        public int DeckCount { get; private set; }

        public PredictionEventArgs(List<Deck> potentialDecks)
        {
            PotentialDecks = potentialDecks;
            DeckCount = PotentialDecks.Count;
        }
    }
}
