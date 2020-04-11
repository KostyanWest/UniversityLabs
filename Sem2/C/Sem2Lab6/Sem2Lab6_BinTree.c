#include <stdio.h>
#include <stdlib.h>
#include <errno.h>

#include "Sem2Lab6_BinTree.h"

BinTreeElement* CreateBinTreeElement (int value)
{
	BinTreeElement* result = malloc(sizeof(BinTreeElement));
	if (result) {
		result->value = value;
		result->pLeft = NULL;
		result->pRight = NULL;
	} else {
		errno = ENOMEM;
	}
	return result;
}

void DeleteBinTreeElementHierarchy (BinTreeElement* pElement)
{
	if (pElement->pLeft) {
		DeleteBinTreeElementHierarchy(pElement->pLeft);
	}
	if (pElement->pRight) {
		DeleteBinTreeElementHierarchy(pElement->pRight);
	}
	free(pElement);
}

char AddChildToBinSearchTree (BinTreeElement* pRoot, BinTreeElement* pElement)
{
	if (pElement->value < pRoot->value) {
		if (pRoot->pLeft) {
			return AddChildToBinSearchTree(pRoot->pLeft, pElement);
		} else {
			pRoot->pLeft = pElement;
			return 1;
		}
	} else if (pElement->value > pRoot->value) {
		if (pRoot->pRight) {
			return AddChildToBinSearchTree(pRoot->pRight, pElement);
		} else {
			pRoot->pRight = pElement;
			return 1;
		}
	} else {
		errno = EINVAL;
		return 0;
	}
}