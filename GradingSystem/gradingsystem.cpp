#include <iostream>
#include <string>
#include <conio.h>
using namespace std;

int main() {
    string sln, name;
    double q1_pre, q2_pre, q1_mid, q2_mid, q1_fin, q2_fin;
    double act_pre, act_mid, act_fin;
    double exam_pre, exam_mid, exam_fin;

    cout << "Enter SLN: ";
    cin >> sln;
    cin.ignore();
    cout << "Enter Full Name: ";
    getline(cin, name);

    cout << "Enter Quiz 1 (Prelim): ";
    cin >> q1_pre;
    cout << "Enter Quiz 2 (Prelim): ";
    cin >> q2_pre;
    cout << "Enter Quiz 1 (Midterm): ";
    cin >> q1_mid;
    cout << "Enter Quiz 2 (Midterm): ";
    cin >> q2_mid;
    cout << "Enter Quiz 1 (Finals): ";
    cin >> q1_fin;
    cout << "Enter Quiz 2 (Finals): ";
    cin >> q2_fin;

    cout << "Enter Activity (Prelim): ";
    cin >> act_pre;
    cout << "Enter Activity (Midterm): ";
    cin >> act_mid;
    cout << "Enter Activity (Finals): ";
    cin >> act_fin;

    cout << "Enter Prelim Exam: ";
    cin >> exam_pre;
    cout << "Enter Midterm Exam: ";
    cin >> exam_mid;
    cout << "Enter Finals Exam: ";
    cin >> exam_fin;

    double TQuiz = (q1_pre + q2_pre + q1_mid + q2_mid + q1_fin + q2_fin) / 6.0;
    double TActivity = (act_pre + act_mid + act_fin) / 3.0;
    double TMajorExam = (exam_pre + exam_mid + exam_fin) / 3.0;

    double LMS = (TQuiz * 0.5) + (TMajorExam * 0.5);
    double F2F = (TActivity * 0.5) + (TMajorExam * 0.5);
    double FinalGrade = (LMS * 0.5) + (F2F * 0.5);

    cout << "\n==============================\n";
    cout << "SLN: " << sln << endl;
    cout << "Full Name: " << name << endl;
    cout << "Your Grade: " << FinalGrade << endl;
    cout << "Grade Status: " << (FinalGrade >= 75 ? "PASSED" : "FAILED") << endl;
    cout << "==============================\n";

    cout << "Press any key to exit...";
    _getch();
    return 0;
}
