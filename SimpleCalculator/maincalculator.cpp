#include <iostream>
#include <limits>
using namespace std;

int main() {
    string name;
    int num1, num2, sum;

    cout << "Enter your name: ";
    cin >> name;

    cout << "Hello, " << name << "! Let's add two numbers." << endl;

    cout << "Enter the first number: ";
    cin >> num1;

    cout << "Enter the second number: ";
    cin >> num2;

    sum = num1 + num2;

    cout << "The sum of " << num1 << " and " << num2 << " is " << sum << "." << endl;

    if (sum > 0) {
        cout << "The sum is positive." << endl;
    } else if (sum == 0) {
        cout << "The sum is zero." << endl;
    } else {
        cout << "The sum is negative." << endl;
    }

    cout << "Press Enter to exit...";
    cin.ignore(numeric_limits<streamsize>::max(), '\n'); 
    cin.get(); 

    return 0;
}
