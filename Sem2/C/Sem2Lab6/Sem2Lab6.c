/*
Lang	C
Sem		2
Lab		6
Task	5.2 (13)
*/

#include <stdio.h>
#include <stdlib.h>
#include <conio.h>
#include <errno.h>

#include "Sem2Lab6_BinTree.h"
#include "Sem2Lab6_EPN.h"
#include "Sem2Lab6_EMNIA.h"

void OutputTree (BinTreeElement* pElement, int nestingLevel)
{
	if (pElement->pRight) {
		OutputTree (pElement->pRight, nestingLevel + 1);
	}
	for (int i = 0; i < nestingLevel; i++) {
		printf("      ");
	}
	printf("%6d\n", pElement->value);
	if (pElement->pLeft) {
		OutputTree (pElement->pLeft, nestingLevel + 1);
	}
}

int main(void)
{
	int count;
	int* pArr;
	BinTreeElement* pTreeRoot;

	/* Enter size and numbers of queue */
	printf("-> Enter positive number of Tree elements:\n");
	EnterPositiveNum(&count);
	pArr = malloc(count * sizeof(int));
	if (!pArr) {
		printf("Error: out of memory!\n");
		_getch();
		return -1;
	}
	printf("-> Enter numbers of Tree:\n");
	EnterMultiNumInArray(count, pArr);

	/* Filling Tree*/
	pTreeRoot = CreateBinTreeElement(pArr[0]);
	if (!pTreeRoot) {
		free(pArr);
		printf("Error: out of memory!\n");
		_getch();
		return -1;
	}
	for (int i = 1; i < count; i++) {
		BinTreeElement* pElement = CreateBinTreeElement(pArr[i]);
		if (!pElement) {
			DeleteBinTreeElementHierarchy(pTreeRoot);
			free(pArr);
			printf("Error: out of memory!\n");
			_getch();
			return -1;
		}
		if (!AddChildToBinSearchTree(pTreeRoot, pElement)) {
			free(pElement);
		}
	}
	free(pArr);

	/* Output Tree */
	printf("BinTree:\n");
	OutputTree(pTreeRoot, 0);

	/* Exit */
	DeleteBinTreeElementHierarchy(pTreeRoot);
	printf("-> Press any key to exit.\n");
	_getch();
	return 0;
}