CREATE TABLE IF NOT EXISTS checkpoints
(
    id                  BIGINT          NOT NULL    AUTO_INCREMENT,
    checkpoint_id       VARCHAR(500)    NOT NULL,
    checkpoint_id_hash  BINARY(20)      NOT NULL,
    checkpoint          BIGINT          NULL,
    CONSTRAINT pk_checkpoints PRIMARY KEY (id),
    CONSTRAINT uq_checkpoints_checkpoint_id_hash UNIQUE KEY (checkpoint_id_hash)
) ENGINE = InnoDB
  CHARACTER SET utf8mb4
  COLLATE utf8mb4_unicode_ci;