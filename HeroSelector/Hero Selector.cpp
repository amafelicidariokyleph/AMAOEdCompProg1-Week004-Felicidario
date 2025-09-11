#include <iostream>
#include <string>
#include <algorithm>

int main() {
    std::string character;
    std::string choice;

    std::cout << "Welcome to the Hero Selector!\n";

    do {
        std::cout << "\nChoose your character class (warrior, mage, rogue, healer): ";
        std::getline(std::cin, character);

        std::transform(character.begin(), character.end(), character.begin(), ::tolower);

        if (character == "warrior") {
            std::cout << "A mighty Warrior! Strong and brave, you charge into battle with your sword held high!\n";
        } else if (character == "mage") {
            std::cout << "A wise Mage! Master of the arcane arts, your spells will turn the tide of any fight!\n";
        } else if (character == "rogue") {
            std::cout << "A sneaky Rogue! Quick and cunning, you strike from the shadows and vanish without a trace!\n";
        } else if (character == "healer") {
            std::cout << "A compassionate Healer! Your magic mends wounds and keeps your team fighting strong!\n";
        } else {
            std::cout << "Hmm... a mysterious choice! You must be a legendary hero from a secret class!\n";
            std::cout << "Prepare for an epic adventure unlike any other!\n";
        }

        std::cout << "\nWould you like to choose another character? (yes/no): ";
        std::getline(std::cin, choice);
        std::transform(choice.begin(), choice.end(), choice.begin(), ::tolower);

        while (choice != "yes" && choice != "no") {
            std::cout << "Please enter 'yes' or 'no': ";
            std::getline(std::cin, choice);
            std::transform(choice.begin(), choice.end(), choice.begin(), ::tolower);
        }

    } while (choice == "yes");

    std::cout << "\nGood luck on your quest, brave adventurer!\n";

    std::cout << "Press Enter to exit...";
    std::cin.get();

    return 0;
}
