#include <stdio.h>
#include <stdlib.h>

void EnterMultiNumInArray(int count, int* arr)
{
	int n;
	char ch = 0;
	int correct;

	while (1) {
		int i = 0;
		for (; i < count; i++) {
			correct = scanf_s("%5d%c", &n, &ch, 1);
			if (correct > 0 && ((ch == ' ') || (i == count - 1 && ch == '\n'))) {
				/* Set value */
				arr[i] = n;
			} else {
				break;
			}
		}
		if (ch != '\n') {
			scanf_s("%*[^\n]"); /* Clearing the buffer */
		}
		if (i == count) {
			break;
		}
		printf("-> Invalid input, try again:\n");
	}
}