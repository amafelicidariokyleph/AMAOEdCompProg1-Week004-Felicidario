menu = {
    "chicken": 5.00,
    "soft drink": 2.00,
    "fries": 3.00,
    "burger": 4.50
}

balance = float(input("Enter your initial balance: $"))

done_ordering = False
cart = []

while not done_ordering:
    print("\nMenu:")
    for item, price in menu.items():
        print(f"{item.capitalize()}: ${price:.2f}")
    
    if balance == 0:
        print("\nYour balance is zero. You cannot make any more purchases.")
        break

    item = input("\nWhat would you like to add to your cart? (Choose an item from the menu) ").lower()
    
    if item in menu:
        quantity = int(input(f"How many {item} would you like to add? "))
        item_total = menu[item] * quantity
        
        if item_total <= balance:
            cart.append({"item": item.capitalize(), "quantity": quantity, "price": menu[item]})
            balance -= item_total
            print(f"\nYou added {quantity} x {item}(s) to your cart. Total: ${item_total:.2f}")
        else:
            print("\nNot enough balance for this item.")
    else:
        print("Sorry, that item is not on the menu.")
    
    print(f"\nCurrent balance: ${balance:.2f}")

    done_ordering = input("\nDo you want to add another item? (yes/no): ").lower() == "no"

total_cost = 0
print("\nYour order summary:")
for order in cart:
    item_total = order["price"] * order["quantity"]
    total_cost += item_total
    print(f"{order['quantity']} x {order['item']} - ${item_total:.2f}")

print(f"\nTotal cost: ${total_cost:.2f}")
print(f"Remaining balance: ${balance:.2f}")
print("Thank you for your order!")

input("\nPress Enter to exit...")
