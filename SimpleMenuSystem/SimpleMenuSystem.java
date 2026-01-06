import java.util.*;

public class SimpleMenuSystem {

    public static void main(String[] args) {
        Scanner scanner = new Scanner(System.in);

        Map<String, Double> menu = new LinkedHashMap<>();
        menu.put("chicken", 5.00);
        menu.put("soft drink", 2.00);
        menu.put("fries", 3.00);
        menu.put("burger", 4.50);

        List<Map<String, Object>> cart = new ArrayList<>();

        System.out.print("Enter your initial balance: $");
        double balance = scanner.nextDouble();
        scanner.nextLine(); 

        boolean doneOrdering = false;

        while (!doneOrdering) {
            System.out.println("\nMenu:");
            for (Map.Entry<String, Double> entry : menu.entrySet()) {
                System.out.printf("%s: $%.2f%n",
                        capitalize(entry.getKey()), entry.getValue());
            }

            if (balance == 0) {
                System.out.println("\nYour balance is zero. You cannot make any more purchases.");
                break;
            }

            System.out.print("\nWhat would you like to add to your cart? ");
            String item = scanner.nextLine().toLowerCase();

            if (menu.containsKey(item)) {
                System.out.print("How many " + item + " would you like to add? ");
                int quantity = scanner.nextInt();
                scanner.nextLine(); 

                double itemTotal = menu.get(item) * quantity;

                if (itemTotal <= balance) {
                    Map<String, Object> order = new HashMap<>();
                    order.put("item", capitalize(item));
                    order.put("quantity", quantity);
                    order.put("price", menu.get(item));
                    cart.add(order);

                    balance -= itemTotal;
                    System.out.printf(
                            "\nYou added %d x %s(s) to your cart. Total: $%.2f%n",
                            quantity, item, itemTotal
                    );
                } else {
                    System.out.println("\nNot enough balance for this item.");
                }
            } else {
                System.out.println("Sorry, that item is not on the menu.");
            }

            System.out.printf("\nCurrent balance: $%.2f%n", balance);

            System.out.print("\nDo you want to add another item? (yes/no): ");
            doneOrdering = scanner.nextLine().equalsIgnoreCase("no");
        }

        double totalCost = 0;
        System.out.println("\nYour order summary:");
        for (Map<String, Object> order : cart) {
            int quantity = (int) order.get("quantity");
            double price = (double) order.get("price");
            double itemTotal = quantity * price;
            totalCost += itemTotal;

            System.out.printf("%d x %s - $%.2f%n",
                    quantity, order.get("item"), itemTotal);
        }

        System.out.printf("\nTotal cost: $%.2f%n", totalCost);
        System.out.printf("Remaining balance: $%.2f%n", balance);
        System.out.println("Thank you for your order!");

        System.out.print("\nPress Enter to exit...");
        scanner.nextLine();
        scanner.close();
    }

    private static String capitalize(String word) {
        return word.substring(0, 1).toUpperCase() + word.substring(1);
    }
}
