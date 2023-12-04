// SPDX-License-Identifier: MIT
pragma solidity ^0.8.0;

contract AnimalBetRacev1 {
    enum RaceStatus {
        NotStarted,
        Started,
        Ended
    }
    RaceStatus public raceStatus = RaceStatus.NotStarted;

    uint256 public raceEndTime;
    uint256 public winnerHorseId;
    uint256 public poolAmount;
    uint256 public totalPurchaseOrder;

    mapping(address => bool) public isOwner;
    uint256 public ownerCount = 0;

    event OwnerAdded(address indexed newOwner);
    event OwnerRemoved(address indexed oldOwner);
    event Withdrawn(address indexed byOwner, uint256 amount);

    uint256 private seed;

    event OwnershipTransferred(
        address indexed previousOwner,
        address indexed newOwner
    );

    struct Bet {
        address payable bettor;
        uint256 amount;
    }

    mapping(uint256 => Bet[]) public bets; // horseId => Bet[]

    modifier onlyOwner() {
        require(isOwner[msg.sender], "Caller is not an owner");
        _;
    }

    modifier onlyWhenRaceStarted() {
        require(raceStatus == RaceStatus.Started, "Race not started");
        _;
    }

    constructor() {
        //owner = payable(msg.sender); // Set the contract deployer as the owner
        // Adding the deployer as the first owner.
        isOwner[msg.sender] = true;
        ownerCount++;
        seed = block.timestamp;
        emit OwnerAdded(msg.sender);
    }

    // Function to generate a pseudo-random number between 0 and 9 based on user input
    function generateRandomNumber(
        uint256 userProvidedNumber,
        uint256 userProvidedNumberMultiply
    ) external view returns (uint256) {
        // Ensure the userProvidedNumber is within the specified range
        require(
            userProvidedNumber >= 100000 && userProvidedNumber <= 99999999,
            "Invalid input range"
        );
        // Ensure the userProvidedNumber is within the specified range
        require(
            userProvidedNumberMultiply >= 999 &&
                userProvidedNumberMultiply <= 9999,
            "Invalid input range"
        );

        // Update the seed using a combination of user input and block information
        uint256 seedext = uint256(
            keccak256(
                abi.encodePacked(
                    blockhash(block.number - 1),
                    (block.gaslimit * userProvidedNumberMultiply),
                    block.number,
                    userProvidedNumber,
                    block.difficulty,
                    block.timestamp,
                    msg.sender,
                    block.coinbase,
                    gasleft(),
                    seed
                )
            )
        );

        // Generate a random number between 1 and 10
        uint256 randomResult = (seedext % 10) + 1;

        return randomResult;
    }

    function buyCoins(int256 _itemId) external payable {
        require(msg.value > 0, "Not enough balance");
        if (_itemId > -1) totalPurchaseOrder++;
    }

    function startRace(uint256 _time) external payable onlyOwner {
        require(raceStatus == RaceStatus.NotStarted, "Race already started");
        require(msg.value > 0, "Must bet some amount");
        require((_time * 60) > (6 * 60), "It should be more than 5 mins");

        poolAmount += msg.value;
        raceStatus = RaceStatus.Started;
        raceEndTime = block.timestamp + (_time * 60);

        seed = (
            uint256(
                keccak256(
                    abi.encodePacked(
                        blockhash(block.number - 1),
                        block.difficulty,
                        block.timestamp,
                        msg.sender,
                        block.coinbase
                    )
                )
            )
        );
    }

    function placeBet(uint256 horseId) external payable onlyWhenRaceStarted {
        require(
            raceStatus == RaceStatus.Started,
            "Race not started or already ended"
        );
        require(
            block.timestamp < raceEndTime - 5 minutes,
            "Race already ended or about to end"
        );
        require(msg.value > 0, "Must bet some amount");

        seed = (
            uint256(
                keccak256(
                    abi.encodePacked(
                        blockhash(block.number - 1),
                        block.difficulty,
                        block.timestamp,
                        msg.sender,
                        block.coinbase
                    )
                )
            )
        );

        poolAmount += msg.value;
        bets[horseId].push(
            Bet({bettor: payable(msg.sender), amount: msg.value})
        );
    }

    function endRace(uint256 _winnerHorseId) external onlyOwner {
        require(
            raceStatus == RaceStatus.Started,
            "Race not started or already ended"
        );
        raceStatus = RaceStatus.Ended;
        winnerHorseId = _winnerHorseId;

        distributeWinnings();
        resetRace();
    }

    function distributeWinnings() private {
        uint256 totalWinningBets = bets[winnerHorseId].length;
        if (totalWinningBets == 0) return; // No winners, no distribution required

        uint256 individualWinningAmount = poolAmount / totalWinningBets;

        for (uint256 i = 0; i < totalWinningBets; i++) {
            address payable winnerAddress = bets[winnerHorseId][i].bettor;
            winnerAddress.transfer(individualWinningAmount);
        }
    }

    function GetCurrentTime() public view returns (uint256 _result) {
        return _result = block.timestamp;
    }

    function resetRace() private {
        for (uint256 i = 0; i < 10; i++) {
            // Assuming a maximum of 10 horses
            delete bets[i];
        }
        raceStatus = RaceStatus.NotStarted;
        poolAmount = 0;
    }

    // Function to allow the owner to withdraw all Ether from the contract
    function withdrawFund(uint256 amount) public onlyOwner {
        require(
            address(this).balance >= amount,
            "Insufficient balance in contract"
        );
        payable(msg.sender).transfer(amount);
        emit Withdrawn(msg.sender, amount);
    }

    function addOwner(address payable newOwner) public onlyOwner {
        require(newOwner != address(0), "Invalid address");
        require(!isOwner[newOwner], "Address is already an owner");
        isOwner[newOwner] = true;
        ownerCount++;
        emit OwnerAdded(newOwner);
    }

    function removeOwner(address payable oldowner) public onlyOwner {
        require(oldowner != address(0), "Invalid address");
        require(isOwner[oldowner], "Address is not an owner");
        // An owner should not be able to remove themselves to prevent having zero owners.
        require(oldowner != msg.sender, "Owner cannot remove themselves");
        isOwner[oldowner] = false;
        ownerCount--;
        emit OwnerRemoved(oldowner);
    }
}
